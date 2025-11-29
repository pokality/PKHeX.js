using PKHeX.Api;
using PKHeX.Core;
using System.Text.Json;
using Xunit;

namespace PKHeX.Tests;

[Collection("SaveFile")]
public class HandleOverflowTests
{
    private readonly string _validGen3SavePath = Path.Combine("TestData", "emerald.sav");

    [Fact]
    public void CreateHandle_WithMultipleLoads_CreatesUniqueHandles()
    {
        var saveData = File.ReadAllBytes(_validGen3SavePath);
        var base64Data = Convert.ToBase64String(saveData);

        var handles = new List<int>();

        // Create multiple handles
        for (int i = 0; i < 5; i++)
        {
            var loadResult = PKHeXApi.LoadSave(base64Data);
            var loadResponse = TestHelpers.ToJsonElement(loadResult);
            var handle = loadResponse.GetProperty("handle").GetInt32();
            handles.Add(handle);
        }

        // Verify all handles are unique
        Assert.Equal(handles.Count, handles.Distinct().Count());

        // Clean up
        foreach (var handle in handles)
        {
            PKHeXApi.DisposeSave(handle);
        }
    }

    [Fact]
    public void GetActiveHandleCount_AfterLoads_ReturnsCorrectCount()
    {
        // Clear any existing handles first
        SaveFileManager.Clear();

        var saveData = File.ReadAllBytes(_validGen3SavePath);
        var base64Data = Convert.ToBase64String(saveData);

        // Create 3 handles
        var handles = new List<int>();
        for (int i = 0; i < 3; i++)
        {
            var loadResult = PKHeXApi.LoadSave(base64Data);
            var loadResponse = TestHelpers.ToJsonElement(loadResult);
            handles.Add(loadResponse.GetProperty("handle").GetInt32());
        }

        // Check active count
        var countResult = PKHeXApi.GetActiveHandleCount();
        var countResponse = TestHelpers.ToJsonElement(countResult);
        var count = countResponse.GetProperty("count").GetInt32();

        Assert.Equal(3, count);

        // Dispose one
        PKHeXApi.DisposeSave(handles[0]);

        // Check count again
        countResult = PKHeXApi.GetActiveHandleCount();
        countResponse = TestHelpers.ToJsonElement(countResult);
        count = countResponse.GetProperty("count").GetInt32();

        Assert.Equal(2, count);

        // Clean up remaining
        PKHeXApi.DisposeSave(handles[1]);
        PKHeXApi.DisposeSave(handles[2]);
    }

    [Fact]
    public void DisposeSave_WithSameHandleTwice_SecondCallFails()
    {
        var saveData = File.ReadAllBytes(_validGen3SavePath);
        var base64Data = Convert.ToBase64String(saveData);

        var loadResult = PKHeXApi.LoadSave(base64Data);
        var loadResponse = TestHelpers.ToJsonElement(loadResult);
        var handle = loadResponse.GetProperty("handle").GetInt32();

        // First dispose should succeed
        var disposeResult1 = PKHeXApi.DisposeSave(handle);
        Assert.True(TestHelpers.IsSuccess(disposeResult1));

        // Second dispose should fail
        var disposeResult2 = PKHeXApi.DisposeSave(handle);
        Assert.True(TestHelpers.IsError(disposeResult2));
    }

    [Fact]
    public void GetSave_AfterDispose_ReturnsError()
    {
        var saveData = File.ReadAllBytes(_validGen3SavePath);
        var base64Data = Convert.ToBase64String(saveData);

        var loadResult = PKHeXApi.LoadSave(base64Data);
        var loadResponse = TestHelpers.ToJsonElement(loadResult);
        var handle = loadResponse.GetProperty("handle").GetInt32();

        // Dispose the save
        PKHeXApi.DisposeSave(handle);

        // Try to get info - should fail
        var infoResult = PKHeXApi.GetSaveInfo(handle);
        Assert.True(TestHelpers.IsError(infoResult));
    }

    [Fact]
    public void HandleRecycling_AfterClear_StartsFromOne()
    {
        // Clear all handles
        SaveFileManager.Clear();

        var saveData = File.ReadAllBytes(_validGen3SavePath);
        var base64Data = Convert.ToBase64String(saveData);

        // First load after clear should get handle 1
        var loadResult = PKHeXApi.LoadSave(base64Data);
        var loadResponse = TestHelpers.ToJsonElement(loadResult);
        var handle = loadResponse.GetProperty("handle").GetInt32();

        Assert.Equal(1, handle);

        // Clean up
        PKHeXApi.DisposeSave(handle);
    }

    [Fact]
    public void ConcurrentHandleCreation_CreatesUniqueHandles()
    {
        // Clear to start fresh
        SaveFileManager.Clear();

        var saveData = File.ReadAllBytes(_validGen3SavePath);
        var base64Data = Convert.ToBase64String(saveData);

        var handles = new System.Collections.Concurrent.ConcurrentBag<int>();
        var tasks = new List<Task>();

        // Create 10 handles concurrently
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                var loadResult = PKHeXApi.LoadSave(base64Data);
                var loadResponse = TestHelpers.ToJsonElement(loadResult);
                var handle = loadResponse.GetProperty("handle").GetInt32();
                handles.Add(handle);
            }));
        }

        Task.WaitAll(tasks.ToArray());

        // Verify all handles are unique
        var handleList = handles.ToList();
        Assert.Equal(handleList.Count, handleList.Distinct().Count());

        // Clean up
        foreach (var handle in handleList)
        {
            PKHeXApi.DisposeSave(handle);
        }
    }

    [Fact]
    public void ConcurrentDisposeAndCreate_HandlesCorrectly()
    {
        SaveFileManager.Clear();

        var saveData = File.ReadAllBytes(_validGen3SavePath);
        var base64Data = Convert.ToBase64String(saveData);

        // Create initial handles
        var initialHandles = new List<int>();
        for (int i = 0; i < 5; i++)
        {
            var loadResult = PKHeXApi.LoadSave(base64Data);
            var loadResponse = TestHelpers.ToJsonElement(loadResult);
            initialHandles.Add(loadResponse.GetProperty("handle").GetInt32());
        }

        var newHandles = new System.Collections.Concurrent.ConcurrentBag<int>();
        var tasks = new List<Task>();

        // Concurrently dispose some and create new ones
        for (int i = 0; i < 3; i++)
        {
            var handleToDispose = initialHandles[i];
            tasks.Add(Task.Run(() =>
            {
                PKHeXApi.DisposeSave(handleToDispose);
            }));

            tasks.Add(Task.Run(() =>
            {
                var loadResult = PKHeXApi.LoadSave(base64Data);
                var loadResponse = TestHelpers.ToJsonElement(loadResult);
                newHandles.Add(loadResponse.GetProperty("handle").GetInt32());
            }));
        }

        Task.WaitAll(tasks.ToArray());

        // Check final count
        var countResult = PKHeXApi.GetActiveHandleCount();
        var countResponse = TestHelpers.ToJsonElement(countResult);
        var count = countResponse.GetProperty("count").GetInt32();

        // Should have: 5 initial - 3 disposed + 3 new = 5
        Assert.Equal(5, count);

        // Clean up remaining
        foreach (var handle in initialHandles.Skip(3))
        {
            PKHeXApi.DisposeSave(handle);
        }
        foreach (var handle in newHandles)
        {
            PKHeXApi.DisposeSave(handle);
        }
    }

    [Fact]
    public void RapidCreateDispose_MaintainsConsistency()
    {
        SaveFileManager.Clear();

        var saveData = File.ReadAllBytes(_validGen3SavePath);
        var base64Data = Convert.ToBase64String(saveData);

        // Rapidly create and dispose handles
        for (int i = 0; i < 20; i++)
        {
            var loadResult = PKHeXApi.LoadSave(base64Data);
            var loadResponse = TestHelpers.ToJsonElement(loadResult);
            var handle = loadResponse.GetProperty("handle").GetInt32();

            Assert.True(handle > 0);

            var disposeResult = PKHeXApi.DisposeSave(handle);
            Assert.True(TestHelpers.IsSuccess(disposeResult));
        }

        // Final count should be 0
        var countResult = PKHeXApi.GetActiveHandleCount();
        var countResponse = TestHelpers.ToJsonElement(countResult);
        var count = countResponse.GetProperty("count").GetInt32();

        Assert.Equal(0, count);
    }

    [Fact]
    public void DisposeSave_WithInvalidHandle_ReturnsError()
    {
        var result = PKHeXApi.DisposeSave(-1);
        Assert.True(TestHelpers.IsError(result));
    }

    [Fact]
    public void DisposeSave_WithNonExistentHandle_ReturnsError()
    {
        var result = PKHeXApi.DisposeSave(99999);
        Assert.True(TestHelpers.IsError(result));
    }
}
