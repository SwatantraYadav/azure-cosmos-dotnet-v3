﻿//-----------------------------------------------------------------------
// <copyright file="JsonNewtonsoftNavigator.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Microsoft.Azure.Cosmos.NetFramework.Tests.Json
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Azure.Cosmos.Json;
    using Newtonsoft.Json.Linq;

    internal sealed class JsonNewtonsoftNavigator : IJsonNavigator
    {
        private struct NewtonsoftNode : IJsonNavigatorNode
        {
            public NewtonsoftNode(JToken jToken, JsonNodeType jsonNodeType)
            {
                this.JToken = jToken;
                this.JsonNodeType = jsonNodeType;
            }

            public JToken JToken { get; }
            public JsonNodeType JsonNodeType { get; }
        }

        private readonly NewtonsoftNode root;

        public JsonNewtonsoftNavigator(string input)
        {
            JToken rootJToken = JToken.Parse(input);
            this.root = new NewtonsoftNode(rootJToken, this.JTokenToJsonNodeType(rootJToken));
        }

        public JsonSerializationFormat SerializationFormat
        {
            get
            {
                return JsonSerializationFormat.Text;
            }
        }

        public IJsonNavigatorNode GetArrayItemAt(IJsonNavigatorNode arrayNode, int index)
        {
            JArray jArray = ((NewtonsoftNode)arrayNode).JToken as JArray;
            return new NewtonsoftNode(jArray[index], this.JTokenToJsonNodeType(jArray[index]));
        }

        public int GetArrayItemCount(IJsonNavigatorNode arrayNode)
        {
            JArray jArray = ((NewtonsoftNode)arrayNode).JToken as JArray;
            return jArray.Count;
        }

        public IEnumerable<IJsonNavigatorNode> GetArrayItems(IJsonNavigatorNode arrayNode)
        {
            JArray jArray = ((NewtonsoftNode)arrayNode).JToken as JArray;
            foreach (JToken arrayItem in jArray)
            {
                yield return new NewtonsoftNode(arrayItem, this.JTokenToJsonNodeType(arrayItem));
            }
        }

        private JsonNodeType JTokenToJsonNodeType(JToken jToken)
        {
            switch (jToken.Type)
            {
                case JTokenType.Object:
                    return JsonNodeType.Object;
                case JTokenType.Array:
                    return JsonNodeType.Array;
                case JTokenType.Integer:
                case JTokenType.Float:
                    return JsonNodeType.Number;
                case JTokenType.String:
                    return JsonNodeType.String;
                case JTokenType.Boolean:
                    return ((bool)jToken) ? JsonNodeType.True : JsonNodeType.False;
                case JTokenType.Null:
                case JTokenType.Undefined:
                    return JsonNodeType.Null;
                case JTokenType.Constructor:
                case JTokenType.Property:
                case JTokenType.Comment:
                case JTokenType.Date:
                case JTokenType.Raw:
                case JTokenType.Bytes:
                case JTokenType.Guid:
                case JTokenType.Uri:
                case JTokenType.TimeSpan:
                    return JsonNodeType.String;
                default:
                    throw new InvalidOperationException();
            }
        }

        public JsonNodeType GetNodeType(IJsonNavigatorNode node)
        {
            return ((NewtonsoftNode)node).JsonNodeType;
        }

        public double GetNumberValue(IJsonNavigatorNode numberNode)
        {
            return (double)((NewtonsoftNode)numberNode).JToken;
        }

        public sbyte GetInt8Value(IJsonNavigatorNode numberNode)
        {
            return (sbyte)((NewtonsoftNode)numberNode).JToken;
        }

        public short GetInt16Value(IJsonNavigatorNode numberNode)
        {
            return (short)((NewtonsoftNode)numberNode).JToken;
        }

        public int GetInt32Value(IJsonNavigatorNode numberNode)
        {
            return (int)((NewtonsoftNode)numberNode).JToken;
        }

        public long GetInt64Value(IJsonNavigatorNode numberNode)
        {
            return (long)((NewtonsoftNode)numberNode).JToken;
        }

        public uint GetUInt32Value(IJsonNavigatorNode numberNode)
        {
            return (uint)((NewtonsoftNode)numberNode).JToken;
        }

        public float GetFloat32Value(IJsonNavigatorNode numberNode)
        {
            return (float)((NewtonsoftNode)numberNode).JToken;
        }

        public double GetFloat64Value(IJsonNavigatorNode numberNode)
        {
            return (double)((NewtonsoftNode)numberNode).JToken;
        }

        public Guid GetGuidValue(IJsonNavigatorNode guidNode)
        {
            return (Guid)((NewtonsoftNode)guidNode).JToken;
        }

        public bool TryGetBufferedBinaryValue(IJsonNavigatorNode binaryNode, out IReadOnlyList<byte> bufferedBinaryValue)
        {
            bufferedBinaryValue = null;
            return false;
        }

        public byte[] GetBinaryValue(IJsonNavigatorNode binaryNode)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ObjectProperty> GetObjectProperties(IJsonNavigatorNode objectNode)
        {
            JObject jObject = ((NewtonsoftNode)objectNode).JToken as JObject;
            foreach (KeyValuePair<string, JToken> kvp in jObject)
            {
                yield return new ObjectProperty(
                    new NewtonsoftNode(JToken.FromObject(kvp.Key), JsonNodeType.FieldName),
                    new NewtonsoftNode(kvp.Value, this.JTokenToJsonNodeType(kvp.Value)));
            }
        }

        public int GetObjectPropertyCount(IJsonNavigatorNode objectNode)
        {
            JObject jObject = ((NewtonsoftNode)objectNode).JToken as JObject;
            return jObject.Count;
        }

        public IJsonNavigatorNode GetRootNode()
        {
            return this.root;
        }

        public string GetStringValue(IJsonNavigatorNode stringNode)
        {
            return (string)((NewtonsoftNode)stringNode).JToken;
        }

        public bool TryGetBufferedStringValue(IJsonNavigatorNode stringNode, out IReadOnlyList<byte> bufferedStringValue)
        {
            bufferedStringValue = null;
            return false;
        }

        public bool TryGetObjectProperty(IJsonNavigatorNode objectNode, string propertyName, out ObjectProperty objectProperty)
        {
            objectProperty = default(ObjectProperty);
            JObject jObject = ((NewtonsoftNode)objectNode).JToken as JObject;
            JToken jToken;
            if (jObject.TryGetValue(propertyName, out jToken))
            {
                objectProperty = new ObjectProperty(
                    new NewtonsoftNode(JToken.FromObject(propertyName), JsonNodeType.FieldName),
                    new NewtonsoftNode(jToken, this.JTokenToJsonNodeType(jToken)));
                return true;
            }

            return false;
        }

        public bool TryGetBufferedRawJson(IJsonNavigatorNode jsonNode, out IReadOnlyList<byte> bufferedRawJson)
        {
            bufferedRawJson = null;
            return false;
        }
    }
}
