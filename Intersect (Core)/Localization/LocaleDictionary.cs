using System.Collections;

namespace Intersect.Localization;

public partial class LocaleDictionary<TKey, TValue> : Localized, IDictionary<TKey, TValue> where TValue : Localized
{

    private readonly IDictionary<TKey, TValue> mDefaults;

    private readonly IDictionary<TKey, TValue> mValues;

    private bool mDefaultsFrozen;

    public LocaleDictionary(
        IEnumerable<KeyValuePair<TKey, TValue>> defaults = null,
        IEnumerable<KeyValuePair<TKey, TValue>> values = null
    )
    {
        mDefaults = defaults == null
            ? new SortedDictionary<TKey, TValue>()
            : new SortedDictionary<TKey, TValue>(
                defaults is IDictionary<TKey, TValue> dictionaryDefaults
                    ? dictionaryDefaults
                    : defaults.ToDictionary(pair => pair.Key, pair => pair.Value)
            );

        mValues = values == null
            ? new SortedDictionary<TKey, TValue>()
            : new SortedDictionary<TKey, TValue>(
                values is IDictionary<TKey, TValue> dictionaryValues
                    ? dictionaryValues
                    : values.ToDictionary(pair => pair.Key, pair => pair.Value)
            );
    }

    private ICollection<KeyValuePair<TKey, TValue>> Pairs =>
        Keys.Select(
                key =>
                {
                    if (key == null)
                    {
                        throw new ArgumentNullException(nameof(key));
                    }

                    if (mValues.TryGetValue(key, out var value) || mDefaults.TryGetValue(key, out value))
                    {
                        return new KeyValuePair<TKey, TValue>(key, value);
                    }

                    throw new InvalidOperationException();
                }
            )
            .ToList();

    public TValue this[TKey key]
    {
        get => mValues.TryGetValue(key, out var backingValue) ? backingValue : mDefaults[key];

        set
        {
            if (mDefaultsFrozen || mDefaults.ContainsKey(key))
            {
                mValues[key] = value;
            }
            else
            {
                mDefaults[key] = value;
            }
        }
    }

    public int Count => mDefaults.Count;

    public bool IsReadOnly => true;

    public ICollection<TKey> Keys => mDefaults.Keys;

    public ICollection<TValue> Values => Keys.Select(
            key =>
            {
                if (key == null)
                {
                    throw new InvalidOperationException();
                }

                return this[key];
            }
        )
        .ToList();

    public void Add(TKey key, TValue value)
    {
        if (mDefaultsFrozen || mDefaults.ContainsKey(key))
        {
            mValues.Add(key, value);
        }
        else
        {
            mDefaults.Add(key, value);
        }
    }

    public bool ContainsKey(TKey key)
    {
        return mDefaults.ContainsKey(key);
    }

    public void FreezeDefaults()
    {
        mDefaultsFrozen = true;
    }

    public bool Remove(TKey key)
    {
        return mValues.Remove(key);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return mValues.TryGetValue(key, out value) || mDefaults.TryGetValue(key, out value);
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return Pairs.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        if (mDefaultsFrozen || mDefaults.ContainsKey(item.Key))
        {
            mValues.Add(item);
        }
        else
        {
            mDefaults.Add(item);
        }
    }

    public void Clear()
    {
        mValues.Clear();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return mValues.Contains(item);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        Pairs.CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return mValues.Remove(item);
    }
}
