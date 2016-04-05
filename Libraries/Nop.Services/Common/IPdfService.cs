using System.Collections.Generic;
using System.IO;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.SubscriptionOrders;
using Nop.Core.Domain.Shipping;

namespace Nop.Services.Common
{
    /// <summary>
    /// Customer service interface
    /// </summary>
    public partial interface IPdfService
    {
        /// <summary>
        /// Print an order to PDF
        /// </summary>
        /// <param name="order">SubscriptionOrder</param>
        /// <param name="languageId">Language identifier; 0 to use a language used when placing an order</param>
        /// <returns>A path of generated file</returns>
        string PrintSubscriptionOrderToPdf(SubscriptionOrder order, int languageId);

        /// <summary>
        /// Print orders to PDF
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="orders">SubscriptionOrders</param>
        /// <param name="languageId">Language identifier; 0 to use a language used when placing an order</param>
        void PrintSubscriptionOrdersToPdf(Stream stream, IList<SubscriptionOrder> orders, int languageId = 0);
        /// <summary>
        /// Print an order to PDF
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="languageId">Language identifier; 0 to use a language used when placing an order</param>
        /// <returns>A path of generated file</returns>
        string PrintOrderToPdf(SubscriptionOrder order, int languageId);

        /// <summary>
        /// Print orders to PDF
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="orders">Orders</param>
        /// <param name="languageId">Language identifier; 0 to use a language used when placing an order</param>
        void PrintOrdersToPdf(Stream stream, IList<SubscriptionOrder> orders, int languageId = 0);

        /// <summary>
        /// Print packaging slips to PDF
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="shipments">Shipments</param>
        /// <param name="languageId">Language identifier; 0 to use a language used when placing an order</param>
        void PrintPackagingSlipsToPdf(Stream stream, IList<Shipment> shipments, int languageId = 0);

        
        /// <summary>
        /// Print products to PDF
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="products">Products</param>
        void PrintProductsToPdf(Stream stream, IList<Product> products);
    }
}