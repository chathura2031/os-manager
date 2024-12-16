using System.Text.Json;
using System.Text.Json.Nodes;
using OSManager.Core.Enums;
using OSManager.Core.Extensions;
using OSManager.Daemon.Extensions;

namespace OSManager.Daemon.Packages;

public class Chrome : IPackage
{
    public static readonly Chrome Instance = new();

    public Package Package { get; } = Package.Chrome;

    public List<IPackage> Dependencies { get; } = [];

    public List<IPackage> OptionalExtras { get; } = [];
    
    public int Install(int stage, string data)
    {
        int statusCode = 0;
        switch (stage)
        {
            case 1:
            {
                Utilities.BashStack.PushInstallStage(stage + 1, Package.Name());
                this.InstallDependencies();
                break;
            }
            case 2:
            {
                statusCode = DownloadPackage(2, out string filePath);
                if (statusCode != 0)
                {
                    Console.WriteLine("Failed to download debian file");
                    File.Delete(filePath);
                    return 1;
                }
                
                Utilities.RunInReverse([
                    () => Utilities.BashStack.PushBashCommand($"apt install -y --fix-broken {filePath}", true),
                    () => Utilities.BashStack.PushInstallStage(stage + 1, Package.Name(), filePath),
                ]);
                break;
            }
            case 3:
            {
                statusCode = DeletePackage(2, data);
                if (statusCode != 0)
                {
                    Console.WriteLine("Failed to delete debian file");
                    return 1;
                }
                break;
            }
            default:
                throw new ArgumentException($"{Package.PrettyName()} does not have {stage} stages of installation.");
        }

        return statusCode;
    }

    public int Configure(int stage)
    {
        int statusCode = 0;
        switch (stage)
        {
            case 1:
            {
                break;
            }
            default:
                throw new ArgumentException($"{Package.PrettyName()} does not have {stage} stages of configuration.");
        }
        
        return statusCode;
    }

    public int BackupConfig(int stage)
    {
        int statusCode = 0;
        switch (stage)
        {
            case 1:
            {
                Utilities.RunInReverse([
                    () => Utilities.BashStack.PushBashCommand($"{Utilities.EncryptorPath} -f true --workingdir {Utilities.EncryptedBackupDirectory} --action DecryptAll"),
                    () => Utilities.BashStack.PushBackupConfigStage(stage + 1, Package.Name())
                ]);
                break;
            }
            case 2:
            {
                string origin = Path.Join(Utilities.ConfigDirectory, Package.Name());
                string encryptedDestination = Path.Join(Utilities.EncryptedBackupDirectory, Package.Name());
                string templateDir = Path.Join(Utilities.EncryptedBackupDirectory, $"{Package.Name()}-template");
                
                // Backup basic profile data
                CloneTemplate(templateDir, encryptedDestination, origin);
                
                string[] profileNames = GetProfileNames(JsonDocument.Parse(File.ReadAllText(Path.Join(origin, "Local State"))).RootElement);
                foreach (string profileName in profileNames)
                {
                    string profileOrigin = Path.Join(origin, profileName);
                    string encryptedProfileDestination = Path.Join(encryptedDestination, profileName);

                    // Backup the general profile data
                    BackupGeneralProfileData(encryptedProfileDestination, profileOrigin);
                    
                    // Backup bookmark data
                    Utilities.CopyFile(Path.Join(profileOrigin, "Bookmarks"), Path.Join(encryptedProfileDestination, "Bookmarks"));
                    
                    // Backup history
                    Utilities.CopyFile(Path.Join(profileOrigin, "History"), Path.Join(encryptedProfileDestination, "History"));
                    
                    // Backup cookies
                    Utilities.CopyFile(Path.Join(profileOrigin, "Cookies"), Path.Join(encryptedProfileDestination, "Cookies"));
                    
                    // Backup extension data
                    BackupExtensionsForProfile(encryptedProfileDestination, profileOrigin);
                }

                Utilities.BashStack.PushBackupConfigStage(stage + 1, Package.Name());
                break;
            }
            case 3:
            {
                Utilities.BashStack.PushBashCommand($"{Utilities.EncryptorPath} -f true --workingdir {Utilities.EncryptedBackupDirectory} --action EncryptAll");
                break;
            }
            default:
                throw new ArgumentException($"{Package.PrettyName()} does not have {stage} stages of configuration backup.");
        }
        
        return statusCode;
    }
    
    private int DownloadPackage(int verbosity, out string filePath)
    {
        // Download the debian package
        if (verbosity > 0)
        {
            Console.WriteLine("Downloading debian package...");
        }

        int statusCode = Utilities.DownloadFromUrl(
            "https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb", $"{Package.Name()}.deb",
            out filePath, $"/tmp/osman-{Guid.NewGuid()}/");

        return statusCode;
    }
    
    private int DeletePackage(int verbosity, string filePath)
    {
        // Delete the debian package
        if (verbosity > 0)
        {
            Console.WriteLine("Deleting debian package...");
        }
        File.Delete(filePath);

        return 0;
    }

    private string[] GetProfileNames(JsonElement localStateJson)
    {
        if (!localStateJson.TryGetProperty("profile", out JsonElement profileData) || !profileData.TryGetProperty("info_cache", out profileData))
        {
            return [];
        }
        
        string[] profileNames = new string[profileData.GetPropertyCount()];
        int i = 0;
        foreach (JsonProperty jsonProperty in profileData.EnumerateObject())
        {
            profileNames[i] = jsonProperty.Name;
            i++;
        }
    
        return profileNames;
    }

    private string[] GetProfileNames(JsonObject localStateJson)
    {
        JsonObject profileData = (JsonObject)localStateJson["profile"]!["info_cache"]!;

        string[] profileNames = profileData.GetKeys();
        
        return profileNames;
    }

    private void BackupGeneralProfileData(string profileBackupDir, string profileInstallDir)
    {
        string preferencesFilePathA = Path.Join(profileBackupDir, "Preferences");
        string preferencesFilePathB = Path.Join(profileInstallDir, "Preferences");
        JsonObject preferencesJsonA = JsonNode.Parse(File.ReadAllText(preferencesFilePathA))!.AsObject();
        JsonObject preferencesJsonB = JsonNode.Parse(File.ReadAllText(preferencesFilePathB))!.AsObject();

        object[] keysToReplace = [
            new object[]
            {
                "autofill", new object[]
                {
                    "credit_card_enabled", "payment_cvc_storage", "payment_card_benefits", "profile_enabled"
                }
            },
            "accessibility",
            new object[]
            {
                "bookmark_bar", new object[]
                {
                    "show_on_all_tabs"
                }
            },
            new object[]
            {
                "browser", new object[]
                {
                    "enable_spellchecking",
                    new object[]
                    {
                        "theme", new object[]
                        {
                            "is_grayscale", "color_variant", "user_color"
                        }
                    },
                    "show_home_button"
                }
            },
            "credentials_enable_autosignin", "credentials_enable_service",
            new object[]
            {
                "download", new object[]
                {
                    "default_directory", "prompt_for_download"
                }
            },
            new object[]
            {
                "download_bubble", new object[]
                {
                    "partial_view_enabled"
                }
            },
            new object[]
            {
                "extensions", new object[]
                {
                    new object[]
                    {
                        "theme", new object[]
                        {
                            "id", "system_theme"
                        }
                    }
                }
            },
            "enterprise_profile_guid", "homepage", "homepage_is_newtabpage", "https_first_balanced_mode_enabled",
            "https_only_mode_auto_enabled", "https_only_mode_enabled",
            new object[]
            {
                "intl", new object[]
                {
                    "accept_languages", "selected_languages"
                }
            },
            new object[] {
                "profile", new object[] {
                    "avatar_index",
                    new object[]
                    {
                        "content_settings", new object[]
                        {
                            new object[]
                            {
                                "exceptions", new object[]
                                {
                                    "cookies"
                                }
                            }
                        }
                    },
                    "created_by_version", "creation_time",
                    new object[]
                    {
                        "default_content_setting_values", new object[]
                        {
                            "cookies"
                        }
                    },
                    "exit_type", "family_member_role", "managed_user_id", "name", "password_manager_leak_detection",
                    "using_default_avatar", "using_default_name", "using_gaia_avatar", "were_old_google_logins_removed"
                }
            },
            new object[]
            {
                "privacy_sandbox", new object[]
                {
                    new object[]
                    {
                        "m1", new object[]
                        {
                            "topics_enabled", "fledge_enabled", "ad_measurement_enabled"
                        }
                    },
                    new object[]
                    {
                        "topics_consent", new object[]
                        {
                            "consent_given", "last_update_reason", "last_update_time", "text_at_last_update"
                        }
                    }
                }
            },
            new object[]
            {
                "payments", new object[]
                {
                    "can_make_payment_enabled"
                }
            },
            "password_manager", "pinned_tabs",
            new object[]
            {
                "safebrowsing", new object[]
                {
                    "enabled", "enhanced", "esb_enabled_via_tailored_security"
                }
            },
            new object[]
            {
                "savefile", new object[]
                {
                    "default_directory"
                }
            },
            new object[]
            {
                "search", new object[]
                {
                    "suggest_enabled"
                }
            },
            new object[]
            {
                "session", new object[]
                {
                    "restore_on_startup", "startup_urls"
                }
            },
            new object[]
            {
                "side_panel", new object[]
                {
                    "is_right_aligned"
                }
            },
            new object[]
            {
                "spellcheck", new object[]
                {
                    "use_spelling_service", "dictionaries"
                }
            },
            new object[]
            {
                "signin", new object[]
                {
                    "allowed", "allowed_on_next_startup"
                }
            },
            "settings", "session",
            new object[]
            {
                "translate", new object[]
                {
                    "enabled"
                }
            },
            "translate_recent_target", "translate_allowlists", "translate_blocked_languages",
            new object[]
            {
                "url_keyed_anonymized_data_collection", new object[]
                {
                    "enabled"
                }
            }
        ];

        Utilities.UpdateJson(ref preferencesJsonA, preferencesJsonB, keysToReplace);
        
        // TODO: Test this out then merge the 2 getProfileNames functions -- they are different (one is the actual profile name and the other is the folder name)
        File.WriteAllText(preferencesFilePathA, preferencesJsonA.ToJsonString(new ()
        {
            WriteIndented = true,
            IndentSize = 4
        }));
    }

    private void BackupExtensionsForProfile(string profileBackupDir, string profileInstallDir)
    {
        string preferencesFilePathA = Path.Join(profileBackupDir, "Preferences");
        string preferencesFilePathB = Path.Join(profileInstallDir, "Preferences");
        JsonObject preferencesJsonA = JsonNode.Parse(File.ReadAllText(preferencesFilePathA))!.AsObject();
        JsonObject preferencesJsonB = JsonNode.Parse(File.ReadAllText(preferencesFilePathB))!.AsObject();

        // Update preferences file
        if (!preferencesJsonA.ContainsKey("extensions") && !preferencesJsonB.ContainsKey("extensions"))
        {
            return;
        }

        JsonObject extensionsJsonA = preferencesJsonA["extensions"]!.AsObject();
        JsonObject extensionsJsonB = preferencesJsonB["extensions"]!.AsObject();
        object[] keysToReplace = [
            "chrome_url_overrides", "commands", "external_uninstalls", "last_chrome_version", "pinned_extensions", 
            "theme", "alerts", "settings"
        ];

        Utilities.UpdateJson(ref extensionsJsonA, extensionsJsonB, keysToReplace);
        
        File.WriteAllText(preferencesFilePathA, preferencesJsonA.ToJsonString(new ()
        {
            WriteIndented = true,
            IndentSize = 4
        }));
        
        // Update extension directories
        // // TODO: Use this to determine which extension folders to copy
        // string[] extensions = extensionsJsonB["settings"]!.AsObject().GetKeys();

        string[] foldersToReplace = ["Extensions", "Local Extension Settings", "DNR Extension Rules"];
        foreach (string folderToReplace in foldersToReplace)
        {
            string extensionsDirA = Path.Join(profileBackupDir, folderToReplace);
            string extensionsDirB = Path.Join(profileInstallDir, folderToReplace);

            if (Directory.Exists(extensionsDirA))
            {
                Directory.Delete(extensionsDirA, true);
            }
            
            if (Directory.Exists(extensionsDirB))
            {
                HashSet<string> extExclusions = [".ldb"];
                if (folderToReplace != "Local Extension Settings")
                {
                    extExclusions.Add(".log");
                }
                
                Utilities.CopyDirectory(extensionsDirB, extensionsDirA, 
                    extExclusions: extExclusions, fileNameExclusions: ["LOG", "LOG.old"]);
            }
        }
    }

    private void CloneTemplate(string templateDir, string backupDir, string installDir)
    {
        // Create the chrome directory from the template
        if (Directory.Exists(backupDir))
        {
            Directory.Delete(backupDir, true);
            Directory.CreateDirectory(backupDir);
        }
        Utilities.CopyDirectory(templateDir, backupDir);
        
        // Update the Local State file
        JsonObject localStateJsonA = JsonNode.Parse(File.ReadAllText(Path.Join(templateDir, "Local State")))!.AsObject();
        JsonObject localStateJsonB = JsonNode.Parse(File.ReadAllText(Path.Join(installDir, "Local State")))!.AsObject();

        object[] keysToReplace = [
            new object[]
            {
                "background_mode", new object[]
                {
                    "enabled"
                }
            },
            new object[]
            {
                "browser", new object[]
                {
                    "hovercard", "custom_chrome_frame"
                }
            },
            new object[]
            {
                "dns_over_https", new object[]
                {
                    "mode", "templates"
                }
            },
            new object[]
            {
                "hardware_acceleration_mode", new object[]
                {
                    "enabled"
                }
            },
            "hardware_acceleration_mode_previous",
            new object[]
            {
                "performance_tuning", new object[]
                {
                    new object[]
                    {
                        "intervention_notification", new object[]
                        {
                            "enabled"
                        }
                    },
                    new object[]
                    {
                        "discard_ring_treatment", new object[]
                        {
                            "enabled"
                        }
                    },
                    new object[]
                    {
                        "high_efficiency_mode", new object[]
                        {
                            "aggressiveness", "state"
                        }
                    }
                }
            },
            new object[]
            {
                "profile", new object[]
                {
                    "avatar_icon", "is_using_default_avatar", "use_gaia_picture", "info_cache", "picker_shown",
                    "profiles_created", "profiles_order"
                }
            }
        ];
        
        Utilities.UpdateJson(ref localStateJsonA, localStateJsonB, keysToReplace);
        
        File.WriteAllText(Path.Join(backupDir, "Local State"), localStateJsonA.ToJsonString(new ()
        {
            WriteIndented = true,
            IndentSize = 4
        }));

        // Create the profile directories from the template
        string[] profiles = GetProfileNames(localStateJsonB);
        foreach (string profileName in profiles)
        {
            if (profileName == "Default")
            {
                continue;
            }
            
            string sourcePath = Path.Join(templateDir, "Default");
            string destinationPath = Path.Join(backupDir, profileName);
            
            Utilities.CopyDirectory(sourcePath, destinationPath);
        }
    }
}