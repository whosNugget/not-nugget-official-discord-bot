using System;
using System.Collections.Generic;
using System.Text;

namespace NuggetOfficialDiscord.Commands.VoiceHubModule.Wizard
{
    public class WizardPool<T, TResult> where T : EmbedInteractionWizard<TResult> where TResult : struct, IWizardResult
    {

    }
}
