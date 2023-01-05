using Serilog;

namespace UnmistakableAPKInstaller.Tools
{
    /// <summary>
    /// Extension class for chain <see cref="CmdToolsProvider"/>
    /// </summary>
    public static class CmdToolsProviderChainExt
    {
        /// <summary>
        /// Try Add new Tool to current <paramref name="cmdToolsProvider"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmdToolsProvider"></param>
        /// <param name="toolInstance"></param>
        /// <returns></returns>
        public static CmdToolsProvider AddTool<T>(this CmdToolsProvider cmdToolsProvider, T toolInstance) where T : BaseCmdTool
        {
            var toolType = typeof(T);

            if (!cmdToolsProvider.tools.Any(x => x.Value.GetType() == toolType))
            {
                cmdToolsProvider.tools.Add(toolType, toolInstance);
            }
            else
            {
                Log.Debug($"{toolType} Tool already exists");
            }

            return cmdToolsProvider;
        }
    }
}
