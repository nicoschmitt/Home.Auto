using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleHomeAuto.Data;
using Windows.ApplicationModel.Activation;

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
            // Register Voice
            var storageFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///VoiceCommands.xml"));
            await Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(storageFile);

            var scenarios = Context.Instance.Scenarios;
            if (scenarios.Count > 0)
            {
                var setName = "HomeCommandSet_";
                var defs = Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinitionManager.InstalledCommandDefinitions;
                foreach (var set in defs)
                {
                    if (set.Key.StartsWith(setName))
                    {
                        var commands = set.Value;
                        await commands.SetPhraseListAsync("scenario", scenarios.Select(s => s.Title).ToArray());
                    }

                }
                //if (Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinitionManager.InstalledCommandDefinitions.TryGetValue("HomeCommandSet_fr-fr", out commands))
                //{
                //    await commands.SetPhraseListAsync("scenario", scenarios.Select(s => s.Title).ToArray());
                //}
            }
        }
    }
}
