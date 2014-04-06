namespace Stumps.Server
{

    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A class that represents a collection of <see cref="T:Stumps.Server.RuleContract" /> objects.
    /// </summary>
    public class RuleContractCollection : ICollection<RuleContract>
    {

        private readonly List<RuleContract> _contracts;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Server.RuleContractCollection"/> class.
        /// </summary>
        public RuleContractCollection()
        {
            _contracts = new List<RuleContract>();
        }

        /// <summary>
        ///     Gets the number of elements contained in the <see cref="T:Stumps.Server.RuleContractCollection" />.
        /// </summary>
        /// <returns>The number of elements contained in the <see cref="T:Stumps.Server.RuleContractCollection" />.</returns>
        public int Count
        {
            get { return _contracts.Count; }
        }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="T:Stumps.Server.RuleContractCollection" /> is read-only.
        /// </summary>
        /// <returns><c>true</c> if the <see cref="T:Stumps.Server.RuleContractCollection" /> is read-only; otherwise, <c>false</c>.</returns>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        ///     Adds an item to the <see cref="T:Stumps.Server.RuleContractCollection" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:Stumps.Server.RuleContractCollection" />.</param>
        public void Add(RuleContract item)
        {
            _contracts.Add(item);
        }

        /// <summary>
        ///     Removes all items from the <see cref="T:Stumps.Server.RuleContractCollection" />.
        /// </summary>
        public void Clear()
        {
            _contracts.Clear();
        }

        /// <summary>
        ///     Determines whether the <see cref="T:Stumps.Server.RuleContractCollection" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:Stumps.Server.RuleContractCollection" />.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="item" /> is found in the <see cref="T:Stumps.Server.RuleContractCollection" />; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(RuleContract item)
        {
            return _contracts.Contains(item);
        }

        /// <summary>
        ///     Copies the items from the collection to the destination array.
        /// </summary>
        /// <param name="array">The array destination array.</param>
        /// <param name="arrayIndex">Index of the array to begin copying.</param>
        public void CopyTo(RuleContract[] array, int arrayIndex)
        {
            _contracts.CopyTo(array, arrayIndex);
        }

        /// <summary>
        ///     Finds all rules with the specified name.
        /// </summary>
        /// <param name="name">The name of the rule.</param>
        /// <returns>A collection of <see cref="T:Stumps.Server.RuleContract"/> objects.</returns>
        public IEnumerable<RuleContract> FindRuleContractByName(string name)
        {

            return _contracts.Where(rule => rule.RuleName.Equals(name, System.StringComparison.OrdinalIgnoreCase));

        }

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<RuleContract> GetEnumerator()
        {
            return _contracts.GetEnumerator();
        }

        /// <summary>
        ///     Removes the first occurrence of a specific object from the <see cref="T:Stumps.Server.RuleContractCollection" />.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:Stumps.Server.RuleContractCollection" />.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="item" /> was successfully removed from the <see cref="T:Stumps.Server.RuleContractCollection" />; otherwise, <c>false</c>. This method also returns <c>false</c> if <paramref name="item" /> is not found in the original <see cref="T:Stumps.Server.RuleContractCollection" />.
        /// </returns>
        public bool Remove(RuleContract item)
        {
            return _contracts.Remove(item);
        }

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_contracts).GetEnumerator();
        }

    }

}
