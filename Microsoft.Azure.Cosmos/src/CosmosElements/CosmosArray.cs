﻿//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Microsoft.Azure.Cosmos.CosmosElements
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Azure.Cosmos.Json;
    using Microsoft.Azure.Cosmos.Query.Core.ExecutionComponent.Distinct;
    using Microsoft.Azure.Cosmos.Query.Core.Monads;

#if INTERNAL
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1601 // Partial elements should be documented
    public
#else
    internal
#endif
    abstract partial class CosmosArray : CosmosElement, IReadOnlyList<CosmosElement>, IEquatable<CosmosArray>, IComparable<CosmosArray>
    {
        private const uint HashSeed = 2533142560;

        protected CosmosArray()
            : base(CosmosElementType.Array)
        {
        }

        public abstract int Count { get; }

        public abstract CosmosElement this[int index] { get; }

        public override void Accept(ICosmosElementVisitor cosmosElementVisitor)
        {
            if (cosmosElementVisitor == null)
            {
                throw new ArgumentNullException(nameof(cosmosElementVisitor));
            }

            cosmosElementVisitor.Visit(this);
        }

        public override TResult Accept<TResult>(ICosmosElementVisitor<TResult> cosmosElementVisitor)
        {
            if (cosmosElementVisitor == null)
            {
                throw new ArgumentNullException(nameof(cosmosElementVisitor));
            }

            return cosmosElementVisitor.Visit(this);
        }

        public override TResult Accept<TArg, TResult>(ICosmosElementVisitor<TArg, TResult> cosmosElementVisitor, TArg input)
        {
            if (cosmosElementVisitor == null)
            {
                throw new ArgumentNullException(nameof(cosmosElementVisitor));
            }

            return cosmosElementVisitor.Visit(this, input);
        }

        public override bool Equals(CosmosElement cosmosElement)
        {
            if (!(cosmosElement is CosmosArray cosmosArray))
            {
                return false;
            }

            return this.Equals(cosmosArray);
        }

        public bool Equals(CosmosArray cosmosArray)
        {
            if (cosmosArray == null)
            {
                return false;
            }

            if (this.Count != cosmosArray.Count)
            {
                return false;
            }

            IEnumerable<(CosmosElement, CosmosElement)> itemPairs = this.Zip(cosmosArray, (first, second) => (first, second));
            foreach ((CosmosElement thisItem, CosmosElement otherItem) in itemPairs)
            {
                if (thisItem != otherItem)
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            uint hash = HashSeed;

            // Incorporate all the array items into the hash.
            for (int index = 0; index < this.Count; index++)
            {
                CosmosElement arrayItem = this[index];
                hash = MurmurHash3.Hash32(arrayItem.GetHashCode(), hash);
            }

            return (int)hash;
        }

        public int CompareTo(CosmosArray cosmosArray)
        {
            UInt128 hash1 = DistinctHash.GetHash(this);
            UInt128 hash2 = DistinctHash.GetHash(cosmosArray);
            return hash1.CompareTo(hash2);
        }

        public static CosmosArray Create(
            IJsonNavigator jsonNavigator,
            IJsonNavigatorNode jsonNavigatorNode)
        {
            return new LazyCosmosArray(jsonNavigator, jsonNavigatorNode);
        }

        public static CosmosArray Create(IEnumerable<CosmosElement> cosmosElements)
        {
            return new EagerCosmosArray(cosmosElements);
        }

        public abstract IEnumerator<CosmosElement> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public static new CosmosArray CreateFromBuffer(ReadOnlyMemory<byte> buffer)
        {
            return CosmosElement.CreateFromBuffer<CosmosArray>(buffer);
        }

        public static new CosmosArray Parse(string json)
        {
            return CosmosElement.Parse<CosmosArray>(json);
        }

        public static bool TryCreateFromBuffer(ReadOnlyMemory<byte> buffer, out CosmosArray cosmosArray)
        {
            return CosmosElement.TryCreateFromBuffer<CosmosArray>(buffer, out cosmosArray);
        }

        public static bool TryParse(string json, out CosmosArray cosmosArray)
        {
            return CosmosElement.TryParse<CosmosArray>(json, out cosmosArray);
        }

        public static new class Monadic
        {
            public static TryCatch<CosmosArray> CreateFromBuffer(ReadOnlyMemory<byte> buffer)
            {
                return CosmosElement.Monadic.CreateFromBuffer<CosmosArray>(buffer);
            }

            public static TryCatch<CosmosArray> Parse(string json)
            {
                return CosmosElement.Monadic.Parse<CosmosArray>(json);
            }
        }
    }
#if INTERNAL
#pragma warning restore SA1601 // Partial elements should be documented
#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#endif
}
