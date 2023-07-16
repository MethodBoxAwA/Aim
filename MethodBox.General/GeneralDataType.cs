namespace MethodBox.General
{
    public static class GeneralDataType
    {
        /// <summary>
        /// delegate of API handler
        /// </summary>
        /// <typeparam name="T">type of return</typeparam>
        /// <typeparam name="R">type of EventArgs</typeparam>
        /// <param name="sender">calling control</param>
        /// <param name="e">event args</param>
        /// <returns></returns>
        public delegate T APIHandler<T, R>(object sender, R e);
    }
}
