using RSG;
using System;

public interface IMultipleSaveSlotService
{
    int CurrentSlot { get; set; }
    int MaxSlots { get; }

    void Save(int slot, object data);
    IPromise<object> Load(int slot, object defaultData = null);

    void Delete(int slot);

    bool IsSlotEmpty(int slot);

    T GetSlotMetadata<T>(int slot);
}
