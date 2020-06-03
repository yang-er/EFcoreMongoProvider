namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Declares that a member of a navigation property should be denormalized when serializing the property.
    /// </summary>
    public sealed class DenormalizeAttribute : Attribute
    {
        /// <summary>
        /// Specify the names of sub-document members to denormalize when serializing the parent document.
        /// </summary>
        /// <param name="names">Member Names</param>
        public DenormalizeAttribute(params string[] names)
        {
            MemberNames = names ?? Array.Empty<string>();
        }

        /// <summary>
        /// The name of the member to denormalize.
        /// </summary>
        public string[] MemberNames { get; }
    }
}
