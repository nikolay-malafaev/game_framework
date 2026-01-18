using System;
using NUnit.Framework;
using R3;
using UnityEngine;

[SetUpFixture]
public sealed class R3TestBootstrap
{
    private TimeProvider prevTimeProvider;
    private FrameProvider prevFrameProvider;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        prevTimeProvider = ObservableSystem.DefaultTimeProvider;
        prevFrameProvider = ObservableSystem.DefaultFrameProvider;

        ObservableSystem.RegisterUnhandledExceptionHandler(static ex => Debug.LogException(ex));
        ObservableSystem.DefaultTimeProvider = UnityTimeProvider.Update;
        ObservableSystem.DefaultFrameProvider = UnityFrameProvider.Update;
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        ObservableSystem.DefaultTimeProvider = prevTimeProvider;
        ObservableSystem.DefaultFrameProvider = prevFrameProvider;
    }
}