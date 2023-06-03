using PunchoutUtils.Attributes;
using System.Reflection;
using System.Text.Json.Serialization;

namespace PunchoutUtils.Models
{
    public class PunchoutEntry : IPunchoutEntry
    {
        [JsonPropertyName("id")]
        public int Id { get; init; }

        /// <summary>
        /// Description of the item
        /// </summary>
        [JsonPropertyName("description")]
        [FieldName("DESCRIPTION[n]")]
        public string? Description { get; set; }

        /// <summary>
        /// SRM product number of the item
        /// </summary>
        [JsonPropertyName("material_number")]
        [FieldName("MATNR[n]")]
        public string? MaterialNumber { get; set; }

        /// <summary>
        /// Item quantity
        /// </summary>
        [JsonPropertyName("quantity")]
        [FieldName("QUANTITY[n]")]
        public int Quantity { get; set; }

        /// <summary>
        /// Quantity unit for item quantity
        /// </summary>
        [JsonPropertyName("unit")]
        [FieldName("UNIT[n]")]
        public string? Unit { get; set; }

        /// <summary>
        /// Price of an item per price unit
        /// </summary>
        [JsonPropertyName("price")]
        [FieldName("PRICE[n]")]
        public float? Price { get; set; }

        /// <summary>
        /// Item currency
        /// </summary>
        [JsonPropertyName("currency")]
        [FieldName("CURRENCY[n]")]
        public string? Currency { get; set; }

        /// <summary>
        /// Price unit of the item (in whole numbers). If empty, 1 is used
        /// </summary>
        [JsonPropertyName("price_unit")]
        [FieldName("PRICEUNIT[n]")]
        public int PriceUnit { get; set; } = 1;

        /// <summary>
        /// Delivery time of the item in days
        /// </summary>
        [JsonPropertyName("lead_time")]
        [FieldName("LEADTIME[n]")]
        public int? LeadTime { get; set; }

        /// <summary>
        /// Long text for the item
        /// </summary>
        [JsonPropertyName("long_text")]
        [FieldName("LONGTEXT_n:132[]")]
        public string? LongText { get; set; }

        /// <summary>
        /// SRM vendor number (business partner) for the item
        /// </summary>
        [JsonPropertyName("vendor")]
        [FieldName("VENDOR[n]")]
        public string? Vendor { get; set; }

        /// <summary>
        /// Vendor product number for the item
        /// </summary>
        [JsonPropertyName("vendor_material")]
        [FieldName("VENDORMAT[n]")]
        public string? VendorMaterial { get; set; }

        /// <summary>
        /// SRM manufacturer number for the item
        /// </summary>
        [JsonPropertyName("manufacturer_code")]
        [FieldName("MANUFACTCODE[n]")]
        public string? ManufacturerCode { get; set; }

        /// <summary>
        /// Item's manufacturer part number
        /// </summary>
        [JsonPropertyName("manufacturer_material")]
        [FieldName("MANUFACTMAT[n]")]
        public string? ManufacturerMaterial { get; set; }

        /// <summary>
        /// SRM material group for the item
        /// </summary>
        [JsonPropertyName("material_group")]
        [FieldName("MATGROUP[n]")]
        public string? MaterialGroup { get; set; }

        /// <summary>
        /// The item is a service
        /// </summary>
        [JsonPropertyName("is_service")]
        [FieldName("SERVICE[n]")]
        public bool? IsService { get; set; }

        /// <summary>
        /// SRM contract to which the item refers
        /// </summary>
        [JsonPropertyName("contract")]
        [FieldName("CONTRACT[n]")]
        public string? Contract { get; set; }

        /// <summary>
        /// Item within the SRM contract
        /// </summary>
        [JsonPropertyName("contract_item")]
        [FieldName("CONTRACT_ITEM[n]")]
        public string? ContractItem { get; set; }

        /// <summary>
        /// Number of an external bid for this item (as reference for a subsequent purchase order)
        /// </summary>
        [JsonPropertyName("external_quote_id")]
        [FieldName("EXT_QUOTE_ID[n]")]
        public string? ExternalQuoteId { get; set; }

        /// <summary>
        /// Item of external bid
        /// </summary>
        [JsonPropertyName("external_quote_item")]
        [FieldName("EXT_QUOTE_ITEM[n]")]
        public string? ExternalQuoteItem { get; set; }

        /// <summary>
        /// Unique database key for this item in the catalog
        /// </summary>
        [JsonPropertyName("external_product_id")]
        [FieldName("EXT_PRODUCT_ID[n]")]
        public string? ExternalProductId { get; set; }

        /// <summary>
        /// URL of the attachment (the attachment must be accessible for downloading under this URL)
        /// </summary>
        [JsonPropertyName("attachment_url")]
        [FieldName("ATTACHMENT[n]")]
        public Uri? AttachmentUrl { get; set; }

        /// <summary>
        /// Title of the attachment (if this is empty the file name from the URL is used)
        /// </summary>
        [JsonPropertyName("attachment_title")]
        [FieldName("ATTACHMENT_TITLE[n]")]
        public string? AttachmentTitle
        {
            get => attachmentTitle ?? AttachmentUrl?.ToString();
            set => attachmentTitle = value;
        }
        private string? attachmentTitle;

        /// <summary>
        /// Purpose of the attachment. C corresponds here to configuration
        /// </summary>
        [JsonPropertyName("attachment_purpose")]
        [FieldName("ATTACHMENT_PURPOSE[n]")]
        public char? AttachmentPurpose { get; set; }

        /// <summary>
        /// Name of a schema via which it was imported in the SRM server
        /// </summary>
        [JsonPropertyName("external_schema_type")]
        [FieldName("EXT_SCHEMA_TYPE[n]")]
        public string? ExternalSchemaType { get; set; }

        /// <summary>
        /// Unique key for an external category from the schema above, independent of the versionof the schema
        /// </summary>
        [JsonPropertyName("external_category_id")]
        [FieldName("EXT_CATEGORY_ID[n]")]
        public string? ExternalCategoryId { get; set; }

        /// <summary>
        /// Unique key for an external category from the schema above, dependent of the versionof the schema
        /// </summary>
        [JsonPropertyName("external_category")]
        [FieldName("EXT_CATEGORY[n]")]
        public string? ExternalCategory { get; set; }

        /// <summary>
        /// Name of a system in the System Landscape Directory (SLD)
        /// </summary>
        [JsonPropertyName("sld_system_name")]
        [FieldName("SLD_SYS_NAME[n]")]
        public string? SldSystemName { get; set; }

        /// <summary>
        /// User-defined field
        /// </summary>
        [JsonPropertyName("custom_field_1")]
        [FieldName("CUST_FIELD1[n]")]
        public string? CustomField1 { get; set; }

        /// <summary>
        /// User-defined field
        /// </summary>
        [JsonPropertyName("custom_field_2")]
        [FieldName("CUST_FIELD2[n]")]
        public string? CustomField2 { get; set; }

        /// <summary>
        /// User-defined field
        /// </summary>
        [JsonPropertyName("custom_field_3")]
        [FieldName("CUST_FIELD3[n]")]
        public string? CustomField3 { get; set; }

        /// <summary>
        /// User-defined field
        /// </summary>
        [JsonPropertyName("custom_field_4")]
        [FieldName("CUST_FIELD4[n]")]
        public string? CustomField4 { get; set; }

        /// <summary>
        /// User-defined field
        /// </summary>
        [JsonPropertyName("custom_field_5")]
        [FieldName("CUST_FIELD5[n]")]
        public string? CustomField5 { get; set; }

        /// <summary>
        /// Item type root (R), outline (O) or leaf (L). Introduced since SRM 7.0 onwards.
        /// </summary>
        [JsonPropertyName("item_type")]
        [FieldName("ITEM_TYPE[n]")]
        public char? ItemType { get; set; }

        /// <summary>
        /// An integer field that defines what the parent of the current item is in the hierarchy structure. This field will be null for an item
        /// that does not have any parent. Introduced since SRM 7.0 onwards.
        /// </summary>
        [JsonPropertyName("parent_id")]
        [FieldName("PARENT_ID[n]")]
        public int? ParentId { get; set; }

        internal static PropertyInfo[] GetPublicProperties() => typeof(PunchoutEntry).GetProperties().Where(p => p.GetMethod?.IsPublic ?? false).ToArray();
    }
}
