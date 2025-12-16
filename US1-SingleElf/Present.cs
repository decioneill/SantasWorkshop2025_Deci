namespace SantasWorkshop2025
{
    /// <summary>
    /// A Present, can be any number of things.
    /// </summary>
    public class Present : NamedObject
    {
        // Present Name (Id)
        public string Name { get; set; }

        // Machine Name present produced at 
        public string CreatedByMachine { get; set; }

        // Name of Elf delivering to Sleigh
        public string DeliveredByElf { get; set; }

        // Name of Recipient
        public string Family { get; set; }
    }
}