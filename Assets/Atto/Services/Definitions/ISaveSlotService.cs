using RSG;
using System;

public interface ISaveSlotService
{
    event Action onSlotChange;
    int CurrentSlot { get; set; }
    IPromise<int> GetMaxSlots();
    void Save<T>(int slot, string id, T value);
    IPromise<T> Load<T>(int slot, string id, T defaultValue = default(T));
    IPromise<bool> IsSlotEmpty(int slot);
    IPromise<DateTime> GetSlotSaveTime(int slot);
    void AddNotSaveableSlot(int slot);
    IPromise<bool> IsSaveableSlot(int slot);
}
