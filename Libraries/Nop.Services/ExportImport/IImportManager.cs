﻿
using System.IO;

namespace Nop.Services.ExportImport
{
    /// <summary>
    /// Import manager interface
    /// </summary>
    public partial interface IImportManager
    {
        void ImportCategoriesFromXlsx(Stream stream);
        /// <summary>
        /// Import products from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        void ImportProductsFromXlsx(Stream stream);
        void ImportSimpleProductsFromXlsx(Stream stream);
        void ImportGiftCardsFromXlsx(Stream stream);
        void ImportManufacturersFromXlsx(Stream stream);

        /// <summary>
        /// Import newsletter subscribers from TXT file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Number of imported subscribers</returns>
        int ImportNewsletterSubscribersFromTxt(Stream stream);

        /// <summary>
        /// Import states from TXT file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Number of imported states</returns>
        int ImportStatesFromTxt(Stream stream);


        int ImportCitiesFromTxt(Stream stream);

        int ImportLocalitiesFromTxt(Stream stream);
    }
}
