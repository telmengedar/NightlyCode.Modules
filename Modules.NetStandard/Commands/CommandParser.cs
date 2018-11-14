using System.Collections.Generic;
using System.Linq;

namespace NightlyCode.Modules.Commands {

    /// <summary>
    /// parses commands from string
    /// </summary>
    public class CommandParser {

        IEnumerable<string> ParseArguments(string data, int index) {
            int startindex = index;
            while(index < data.Length) {
                switch(data[index]) {
                    case ')':
                        if (index != startindex)
                            yield return data.Substring(startindex, index - startindex);
                        yield break;
                    case ',':
                        if (index != startindex)
                            yield return data.Substring(startindex, index - startindex);
                        startindex = index + 1;
                        break;
                }
                ++index;
            }
            if(index != startindex)
                yield return data.Substring(startindex, index - startindex);
        }

        /// <summary>
        /// parses a command from string
        /// </summary>
        /// <param name="data">string which identifies a command to be executed</param>
        /// <returns>command to be executed</returns>
        public ModuleCommand ParseCommand(string data) {
            ModuleCommand command = new ModuleCommand();

            int startindex = 0;
            int index = 0;
            while(index < data.Length) {
                switch(data[index]) {
                    case '.':
                        command.Module = data.Substring(startindex, index - startindex);
                        startindex = index + 1;
                        break;
                    case '=':
                        command.Type = CommandType.Property;
                        command.Endpoint = data.Substring(startindex, index - startindex).Trim();
                        command.Arguments = ParseArguments(data, index + 1).ToArray();
                        index = data.Length;
                        break;
                    case '(':
                        command.Type = CommandType.Method;
                        command.Endpoint = data.Substring(startindex, index - startindex).Trim();
                        command.Arguments = ParseArguments(data, index + 1).ToArray();
                        index = data.Length;
                        break;
                }
                ++index;
            }

            if(command.Type == CommandType.None)
                return null;

            return command;
        }
    }
}