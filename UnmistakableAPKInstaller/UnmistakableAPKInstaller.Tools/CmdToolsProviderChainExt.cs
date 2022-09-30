using UnmistakableAPKInstaller.Helpers;

namespace UnmistakableAPKInstaller.Tools
{
    public static class CmdToolsProviderChainExt
    {
        public static CmdToolsProvider AddTool<T>(this CmdToolsProvider cmdToolsProvider, T toolInstance) where T : BaseCmdTool
        {
            var toolType = typeof(T);

            if (cmdToolsProvider.tools.Any(x => x.GetType() == toolType))
            {
                cmdToolsProvider.tools.Add(toolType, toolInstance);
            }
            else
            {
                CustomLogger.Log($"{toolType} Tool already exists");
            }

            return cmdToolsProvider;
        }
    }
}
