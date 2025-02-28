using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * 需求：
 * - 可以保证一轮不重复
 * - 可以对其中一张重新抓取
 * - 当保证房间之间不会沉底时，将这个 seq 放在全局
 */
[Obsolete]
public class RandomSequenceOld<T>
{
    private readonly List<T> _library;
    private readonly List<T> _graveyard;
    private readonly List<T> _hand;

    public RandomSequenceOld(IEnumerable<T> cards)
    {
        var deck = cards.ToList();
        RandomUtil.Shuffle(deck);
        _library = deck;
        _graveyard = new List<T>();
        _hand = new List<T>();
    }

    /**
     * 洗牌。循环抽牌前需要先调用这个确保不会抽光
     */
    public void EnsureLibraryCount(int count)
    {
        if (_library.Count + _graveyard.Count < count)
        {
            throw new Exception("impossible sequence (may repeat element, cause problems)");
        }

        if (_library.Count >= count)
        {
            return;
        }

        var remains = _library.ToList();
        _library.Clear();
        _library.AddRange(_graveyard);
        _graveyard.Clear();
        RandomUtil.Shuffle(_library);
        _library.AddRange(remains);
    }

    /**
     * Draw a card
     */
    public T Draw()
    {
        var card = _library.PopLast();
        _hand.Add(card);
        return card;
    }

    public void Discard(T card)
    {
        if (!_hand.Contains(card))
        {
            throw new Exception("card not in hand");
        }

        _graveyard.Add(card);
        _hand.Remove(card);
    }

    public void DiscardAll()
    {
        _graveyard.AddRange(_hand);
        _hand.Clear();
    }

    public bool MoveToTop(Predicate<T> criteria)
    {
        if (_library.TryFindAndTake(criteria, out T toBeMove))
        {
            _library.Add(toBeMove);
            return true;
        }

        if (_graveyard.TryFindAndTake(criteria, out toBeMove))
        {
            _library.Add(toBeMove);
            return true;
        }

        Debug.LogWarning("nothing to move");
        return false;
    }

    public void MoveToTopAll(Predicate<T> criteria)
    {
        List<T> toBeMove = new List<T>();
        toBeMove.AddRange(_library.FindAndTakeAll(criteria));
        toBeMove.AddRange(_graveyard.FindAndTakeAll(criteria));
        RandomUtil.Shuffle(toBeMove);
        _library.AddRange(toBeMove);
    }

    public void MoveToGraveyardAll(Predicate<T> criteria)
    {
        IEnumerable<T> toBeMove = _library.FindAndTakeAll(criteria);
        _graveyard.AddRange(toBeMove);
    }

    public static RandomSequenceOld<T> FromDictionary(Dictionary<T, int> sequenceConfig)
    {
        var list = new List<T>();
        foreach (var pair in sequenceConfig)
        {
            for (var i = 0; i < pair.Value; i++)
            {
                list.Add(pair.Key);
            }
        }

        return new RandomSequenceOld<T>(list);
    }
}