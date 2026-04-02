namespace Models.Core
{
    /// <summary>An interface for a model that creates dynamic temporary nodes</summary>
    public interface IGenerateNodes
    {
        /// <summary>
        /// Generate a list of CommandLanguage commands to create nodes
        /// </summary>
        public List<string> GetCommands();

        /// <summary>
        /// Generate the nodes
        /// </summary>
        /// <param name="relativeDirectory"></param>
        public void GenerateNodes(string relativeDirectory);

        /// <summary>
        /// Cleanup the generated nodes
        /// </summary>
        /// <param name="relativeDirectory"></param>
        public void CleanNodes(string relativeDirectory);
    }
}
