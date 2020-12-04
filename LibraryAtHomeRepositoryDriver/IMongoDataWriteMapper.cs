using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryAtHomeRepositoryDriver
{
    public interface IMongoDataWriteMapper<TCOLL>
    {
        /// <summary>
        /// Default create method for type T
        /// </summary>
        /// <param name="instance">The instance to create</param>
        /// <param name="exError">Out exception object</param>
        /// <returns>Boolean success/failure</returns>
        void Write(TCOLL instance);


        /// <summary>
        /// Default create method for type T
        /// </summary>
        /// <returns>Boolean success/failure</returns>
        Task<IEnumerable<TCOLL>> BulkAsync(IEnumerable<TCOLL> instances);

      

        /// <summary>
        /// Default update method for type T
        /// </summary>
        /// <param name="instance">Object of instance to update</param>
        /// <param name="exError">Out exception object</param>
        /// <returns>Instance of type T</returns>
        TCOLL Update(TCOLL instance);


        /// <summary>
        /// Default delete method for type T 
        /// </summary>
        /// <param name="instance">Object of instance to delete</param>
        /// <param name="exError">Out exception object</param>
        /// <returns>Boolean success/failure</returns>
        void Delete(TCOLL instance);

    }
}