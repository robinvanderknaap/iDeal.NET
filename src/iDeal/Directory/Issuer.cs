namespace iDeal.Directory
{
    public class Issuer
    {
        public int Id { get; private set; }

        public string Name { get; private set; }

        /// <summary>
        /// Issuers are listed in the shortlist or the longlist
        /// </summary>
        public ListType ListType { get; private set; }

        public Issuer(int id, string name, ListType listType)
        {
            Id = id;
            Name = name;
            ListType = listType;
        }
    }
}
