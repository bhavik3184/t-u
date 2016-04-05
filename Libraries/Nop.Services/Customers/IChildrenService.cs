using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Customers;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Children service
    /// </summary>
    public partial interface IChildrenService
    {
        /// <summary>
        /// Deletes a children
        /// </summary>
        /// <param name="children">Children</param>
        void DeleteChildren(Children children);
        
        /// <summary>
        /// Gets all childrens
        /// </summary>
        /// <param name="childrenName">Children name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Childrens</returns>
        IPagedList<Children> GetAllChildrens(string childrenName = "",
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            bool showHidden = false);

        /// <summary>
        /// Gets a children
        /// </summary>
        /// <param name="childrenId">Children identifier</param>
        /// <returns>Children</returns>
        Children GetChildrenById(int childrenId);

        /// <summary>
        /// Inserts a children
        /// </summary>
        /// <param name="children">Children</param>
        void InsertChildren(Children children);

        /// <summary>
        /// Updates the children
        /// </summary>
        /// <param name="children">Children</param>
        void UpdateChildren(Children children);
        

        
    }
}
