using System.Collections.Generic;

namespace LibraryAtHomeRepositoryDriver
{
    public interface IMongoDataReadMapper<TCOLL>
    {
        /// <summary>
        /// Default select method for type T
        /// </summary>
        /// <param name="exError">Out exception object</param>
        /// <returns>List of type T</returns>
        List<TCOLL> Read();


        /// <summary>
        /// Default read method for type T 
        /// </summary>
        /// <param name="instance">Object of instance to read</param>
        /// <param name="exError">Out exception object</param>
        /// <returns>Instance of type T</returns>
        List<TCOLL> Read(TCOLL instance);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        long Count();
    }
}