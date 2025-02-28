using System;
using System.Collections.Generic;
using System.Linq;

public class RandomDeck<T>
{
    private readonly List<T> _library;
    private readonly List<T> _graveyard;

    public int LibraryCount => _library.Count;
    public int GraveyardCount => _graveyard.Count;

    public RandomDeck(IEnumerable<T> cards)
    {
        _library = new List<T>();
        _graveyard = new List<T>();
        _graveyard.AddRange(cards);
    }

    /**
     * this method do not have side effects
     */
    public bool TryDrawNoDiscard(out T card)
    {
        if (LibraryCount == 0)
        {
            card = default;
            return false;
        }

        card = _library.PopLast();
        return true;
    }

    /**
     * this method will put card into graveyard automatically
     */
    public T CycleDraw()
    {
        if (LibraryCount == 0)
        {
            Reset();
        }

        var card = _library.PopLast();
        _graveyard.Add(card);
        return card;
    }

    public void Reset()
    {
        _graveyard.AddRange(_library);
        _library.Clear();
        _library.AddRange(_graveyard);
        _graveyard.Clear();
        RandomUtil.Shuffle(_library);
    }

    public void AddToLibrary(T card)
    {
        _library.Add(card);
    }

    public void AddToGraveyard(T card)
    {
        _graveyard.Add(card);
    }

    public void RemoveFromLibrary(T card)
    {
        _library.Remove(card);
    }

    public void RemoveFromGraveyard(T card)
    {
        _graveyard.Remove(card);
    }

    public bool FindInLibrary(Predicate<T> predicate, out T result)
    {
        result = default;
        foreach (var card in _library.Where(card => predicate(card)))
        {
            result = card;
            return true;
        }

        return false;
    }

    public bool FindInGraveyard(Predicate<T> predicate, out T result)
    {
        result = default;
        foreach (var card in _graveyard.Where(card => predicate(card)))
        {
            result = card;
            return true;
        }

        return false;
    }
}