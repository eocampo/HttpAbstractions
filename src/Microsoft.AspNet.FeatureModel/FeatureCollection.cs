// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Framework.Internal;

namespace Microsoft.AspNet.FeatureModel
{
    public class FeatureCollection : IFeatureCollection
    {
        private readonly IFeatureCollection _defaults;
        private readonly IDictionary<Type, object> _featureByFeatureType = new Dictionary<Type, object>();
        private readonly object _containerSync = new object();
        private volatile int _containerRevision;

        public FeatureCollection()
        {
        }

        public FeatureCollection(IFeatureCollection defaults)
        {
            _defaults = defaults;
        }

        public object GetInterface()
        {
            return GetInterface(null);
        }

        public object GetInterface([NotNull] Type type)
        {
            object feature;
            if (_featureByFeatureType.TryGetValue(type, out feature))
            {
                return feature;
            }

            if (_defaults != null && _defaults.TryGetValue(type, out feature))
            {
                return feature;
            }
            return null;
        }

        void SetInterface([NotNull] Type type, object feature)
        {
            if (feature == null)
            {
                Remove(type);
                return;
            }

            lock (_containerSync)
            {
                _featureByFeatureType[type] = feature;
                _containerRevision++;
            }
        }

        public virtual int Revision
        {
            get { return _containerRevision; }
        }

        public void Dispose()
        {
        }

        public IEnumerator<KeyValuePair<Type, object>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<Type, object> item)
        {
            SetInterface(item.Key, item.Value);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<Type, object> item)
        {
            object value;
            return TryGetValue(item.Key, out value) && Equals(item.Value, value);
        }

        public void CopyTo(KeyValuePair<Type, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<Type, object> item)
        {
            return Contains(item) && Remove(item.Key);
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool ContainsKey([NotNull] Type key)
        {
            return GetInterface(key) != null;
        }

        public void Add([NotNull] Type key, [NotNull] object value)
        {
            if (ContainsKey(key))
            {
                throw new ArgumentException();
            }
            SetInterface(key, value);
        }

        public bool Remove([NotNull] Type key)
        {
            lock (_containerSync)
            {
                if (_featureByFeatureType.Remove(key))
                {
                    _containerRevision++;
                    return true;
                }
                return false;
            }
        }

        public bool TryGetValue([NotNull] Type key, out object value)
        {
            value = GetInterface(key);
            return value != null;
        }

        public object this[Type key]
        {
            get { return GetInterface(key); }
            set { SetInterface(key, value); }
        }

        public ICollection<Type> Keys
        {
            get { throw new NotImplementedException(); }
        }

        public ICollection<object> Values
        {
            get { throw new NotImplementedException(); }
        }
    }
}