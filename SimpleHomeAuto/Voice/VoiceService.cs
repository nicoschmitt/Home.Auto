using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleHomeAuto.Data;
using Windows.ApplicationModel.Activation;
using System.Globalization;
using Windows.ApplicationModel.VoiceCommands;

namespace SimpleHomeAuto.Voice
{
    public class VoiceService
    {
        public static Scenario GetScenarioFromInput(VoiceCommandActivatedEventArgs commandArgs)
        {
            // Get the name of the voice command and the text spoken
            string voiceCommandName = commandArgs.Result.RulePath[0];
            string textSpoken = commandArgs.Result.Text;
            Scenario scenario = null;

            switch (voiceCommandName)
            {
                case "scenarioScenario":
                case "launchScenario":
                    var name = commandArgs.Result.SemanticInterpretation.Properties["scenario"].FirstOrDefault();
                    scenario = Context.Instance.Scenarios.FirstOrDefault(s => s.Title == name);
                    break;
            }

            return scenario;
        }

        public static async Task InstallOrUpdateCortana()
        {
            var countryCode = CultureInfo.CurrentCulture.Name.ToLower();
            if (string.IsNullOrWhiteSpace(countryCode))
            {
                countryCode = "en";
            }
            else
            {
                countryCode = countryCode.Substring(0, 2);
            }

            // Register Voice
            var storageFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Voice/VoiceCommands.xml"));
            await Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(storageFile);

            // Register scenario name for recognition
            var scenarios = Context.Instance.Scenarios;
            if (scenarios.Count > 0)
            {

                var setName = "HomeCommandSet_" + countryCode;
                var defs = VoiceCommandDefinitionManager.InstalledCommandDefinitions;

                VoiceCommandDefinition commands;
                if (defs.TryGetValue(setName, out commands))
                {
                    var phraselist = scenarios.Select(s => s.VoiceCommand).ToArray();
                    await commands.SetPhraseListAsync("scenario", phraselist);
                }
            }
        }
    }
}
