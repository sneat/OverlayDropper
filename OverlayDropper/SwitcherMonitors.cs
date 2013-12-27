using System;
using System.Text;

using BMDSwitcherAPI;

namespace SwitcherPanelCSharp
{
    public delegate void SwitcherEventHandler(object sender, object args);

    class AudioMixerMonitorOutputMonitor : IBMDSwitcherAudioMonitorOutputCallback
    {
        //Events:
        public event SwitcherEventHandler LevelNotificationChanged;
        public event SwitcherEventHandler DimChanged;
        public event SwitcherEventHandler DimLevelChanged;
        public event SwitcherEventHandler GainChanged;
        public event SwitcherEventHandler MonitorEnableChanged;
        public event SwitcherEventHandler MuteChanged;
        public event SwitcherEventHandler SoloChanged;
        public event SwitcherEventHandler SoloInputChanged;

        public AudioMixerMonitorOutputMonitor()
        {
        }

        void IBMDSwitcherAudioMonitorOutputCallback.LevelNotification(double Left, double Right, double PeakLeft, double PeakRight)
        {
            if (LevelNotificationChanged != null)
                LevelNotificationChanged(this, null);
        }

        void IBMDSwitcherAudioMonitorOutputCallback.Notify(_BMDSwitcherAudioMonitorOutputEventType EventType)
        {
            switch (EventType)
            {
                case (_BMDSwitcherAudioMonitorOutputEventType.bmdSwitcherAudioMonitorOutputEventTypeDimChanged):
                    if (DimChanged != null)
                        DimChanged(this, null);
                    break;

                case (_BMDSwitcherAudioMonitorOutputEventType.bmdSwitcherAudioMonitorOutputEventTypeDimLevelChanged):
                    if (DimLevelChanged != null)
                        DimLevelChanged(this, null);
                    break;

                case (_BMDSwitcherAudioMonitorOutputEventType.bmdSwitcherAudioMonitorOutputEventTypeGainChanged):
                    if (GainChanged != null)
                        GainChanged(this, null);
                    break;

                case (_BMDSwitcherAudioMonitorOutputEventType.bmdSwitcherAudioMonitorOutputEventTypeMonitorEnableChanged):
                    if (MonitorEnableChanged != null)
                        MonitorEnableChanged(this, null);
                    break;

                case (_BMDSwitcherAudioMonitorOutputEventType.bmdSwitcherAudioMonitorOutputEventTypeMuteChanged):
                    if (MuteChanged != null)
                        MuteChanged(this, null);
                    break;

                case (_BMDSwitcherAudioMonitorOutputEventType.bmdSwitcherAudioMonitorOutputEventTypeSoloChanged):
                    if (SoloChanged != null)
                        SoloChanged(this, null);
                    break;

                case (_BMDSwitcherAudioMonitorOutputEventType.bmdSwitcherAudioMonitorOutputEventTypeSoloInputChanged):
                    if (SoloInputChanged != null)
                        SoloInputChanged(this, null);
                    break;
            }
        }
    }

    class AudioMixerMonitor : IBMDSwitcherAudioMixerCallback
    {
        //Events:
        public event SwitcherEventHandler ProgramOutLevelNotificationChanged;
        public event SwitcherEventHandler ProgramOutBalanceChanged;
        public event SwitcherEventHandler ProgramOutGainChanged;

        public AudioMixerMonitor()
        {
        }

        void IBMDSwitcherAudioMixerCallback.Notify(_BMDSwitcherAudioMixerEventType EventType)
        {
            switch (EventType)
            {
                case (_BMDSwitcherAudioMixerEventType.bmdSwitcherAudioMixerEventTypeProgramOutBalanceChanged):
                    if (ProgramOutBalanceChanged != null)
                        ProgramOutBalanceChanged(this, null);
                    break;

                case (_BMDSwitcherAudioMixerEventType.bmdSwitcherAudioMixerEventTypeProgramOutGainChanged):
                    if (ProgramOutGainChanged != null)
                        ProgramOutGainChanged(this, null);
                    break;
            }
        }

        void IBMDSwitcherAudioMixerCallback.ProgramOutLevelNotification(double Left, double Right, double PeakLeft, double PeakRight)
        {
            if (ProgramOutLevelNotificationChanged != null)
                ProgramOutLevelNotificationChanged(this, null);
        }
    }

    class AudioInputMonitor : IBMDSwitcherAudioInputCallback
    {
        //Events:
        public event SwitcherEventHandler LevelNotificationChanged;
        public event SwitcherEventHandler BalanceChanged;
        public event SwitcherEventHandler GainChanged;
        public event SwitcherEventHandler IsMixedInChanged;
        public event SwitcherEventHandler MixOptionChanged;

        public AudioInputMonitor()
        {
        }

        void IBMDSwitcherAudioInputCallback.LevelNotification(double Left, double Right, double PeakLeft, double PeakRight)
        {
            if (LevelNotificationChanged != null)
                LevelNotificationChanged(this, null);
        }

        void IBMDSwitcherAudioInputCallback.Notify(_BMDSwitcherAudioInputEventType audioType)
        {
            switch (audioType)
            {
                case (_BMDSwitcherAudioInputEventType.bmdSwitcherAudioInputEventTypeBalanceChanged):
                    if (BalanceChanged != null)
                        BalanceChanged(this, null);
                    break;

                case (_BMDSwitcherAudioInputEventType.bmdSwitcherAudioInputEventTypeGainChanged):
                    if (GainChanged != null)
                        GainChanged(this, null);
                    break;

                case (_BMDSwitcherAudioInputEventType.bmdSwitcherAudioInputEventTypeIsMixedInChanged):
                    if (IsMixedInChanged != null)
                        IsMixedInChanged(this, null);
                    break;

                case (_BMDSwitcherAudioInputEventType.bmdSwitcherAudioInputEventTypeMixOptionChanged):
                    if (MixOptionChanged != null)
                        MixOptionChanged(this, null);
                    break;
            }
        }
    }

    class SwitcherMonitor : IBMDSwitcherCallback
    {
        // Events:
        public event SwitcherEventHandler SwitcherDisconnected;
        public event SwitcherEventHandler DownConvertModeChanged;
        public event SwitcherEventHandler ProductNameChanged;
        public event SwitcherEventHandler VideoModeChanged;

        public SwitcherMonitor()
        {
        }

        void IBMDSwitcherCallback.Notify(_BMDSwitcherEventType eventType)
        {
            if (eventType == _BMDSwitcherEventType.bmdSwitcherEventTypeDisconnected)
            {
                if (SwitcherDisconnected != null)
                    SwitcherDisconnected(this, null);
            }
        }
    }

    class InputMonitor : IBMDSwitcherInputCallback
    {
        // Events:
        public event SwitcherEventHandler AvailableExternalPortTypesChanged;
        public event SwitcherEventHandler CurrentExternalPortTypeChanged;
        public event SwitcherEventHandler InputAvailabilityChanged;
        public event SwitcherEventHandler IsPreviewTalliedChanged;
        public event SwitcherEventHandler IsProgramTalliedChanged;
        public event SwitcherEventHandler LongNameChanged;
        public event SwitcherEventHandler PortTypeChanged;
        public event SwitcherEventHandler ShortNameChanged;

        private IBMDSwitcherInput m_input;
        public IBMDSwitcherInput Input { get { return m_input; } }

        public InputMonitor(IBMDSwitcherInput input)
        {
            m_input = input;
        }

        void IBMDSwitcherInputCallback.PropertyChanged(_BMDSwitcherInputPropertyId propId)
        {
            switch (propId)
            {
                case _BMDSwitcherInputPropertyId.bmdSwitcherInputPropertyIdAvailableExternalPortTypes:
                    if (AvailableExternalPortTypesChanged != null)
                        AvailableExternalPortTypesChanged(this, null);
                    break;

                case _BMDSwitcherInputPropertyId.bmdSwitcherInputPropertyIdCurrentExternalPortType:
                    if (CurrentExternalPortTypeChanged != null)
                        CurrentExternalPortTypeChanged(this, null);
                    break;

                case _BMDSwitcherInputPropertyId.bmdSwitcherInputPropertyIdInputAvailability:
                    if (InputAvailabilityChanged != null)
                        InputAvailabilityChanged(this, null);
                    break;

                case _BMDSwitcherInputPropertyId.bmdSwitcherInputPropertyIdIsPreviewTallied:
                    if (IsPreviewTalliedChanged != null)
                        IsPreviewTalliedChanged(this, null);
                    break;

                case _BMDSwitcherInputPropertyId.bmdSwitcherInputPropertyIdIsProgramTallied:
                    if (IsProgramTalliedChanged != null)
                        IsProgramTalliedChanged(this, null);
                    break;

                case _BMDSwitcherInputPropertyId.bmdSwitcherInputPropertyIdLongName:
                    if (LongNameChanged != null)
                        LongNameChanged(this, null);
                    break;

                case _BMDSwitcherInputPropertyId.bmdSwitcherInputPropertyIdPortType:
                    if (PortTypeChanged != null)
                        PortTypeChanged(this, null);
                    break;

                case _BMDSwitcherInputPropertyId.bmdSwitcherInputPropertyIdShortName:
                    if (ShortNameChanged != null)
                        ShortNameChanged(this, null);
                    break;
            }
        }
    }

    class MixEffectBlockMonitor : IBMDSwitcherMixEffectBlockCallback
    {
        // Events:
        public event SwitcherEventHandler FadeToBlackFramesRemainingChanged;
        public event SwitcherEventHandler FadeToBlackRateChanged;
        public event SwitcherEventHandler InFadeToBlackChanged;
        public event SwitcherEventHandler InputAvailabilityMaskChanged;
        public event SwitcherEventHandler InTransitionChanged;
        public event SwitcherEventHandler PreviewInputChanged;
        public event SwitcherEventHandler PreviewLiveChanged;
        public event SwitcherEventHandler PreviewTransitionChanged;
        public event SwitcherEventHandler ProgramInputChanged;
        public event SwitcherEventHandler TransitionFramesRemainingChanged;
        public event SwitcherEventHandler TransitionPositionChanged;

        public MixEffectBlockMonitor()
        {
        }

        void IBMDSwitcherMixEffectBlockCallback.PropertyChanged(_BMDSwitcherMixEffectBlockPropertyId propId)
        {
            switch (propId)
            {
                case _BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdFadeToBlackFramesRemaining:
                    if (FadeToBlackFramesRemainingChanged != null)
                        FadeToBlackFramesRemainingChanged(this, null);
                    break;

                case _BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdFadeToBlackRate:
                    if (FadeToBlackRateChanged != null)
                        FadeToBlackRateChanged(this, null);
                    break;

                case _BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdInFadeToBlack:
                    if (InFadeToBlackChanged != null)
                        InFadeToBlackChanged(this, null);
                    break;

                case _BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdInputAvailabilityMask:
                    if (InputAvailabilityMaskChanged != null)
                        InputAvailabilityMaskChanged(this, null);
                    break;

                case _BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdInTransition:
                    if (InTransitionChanged != null)
                        InTransitionChanged(this, null);
                    break;

                case _BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdPreviewInput:
                    if (PreviewInputChanged != null)
                        PreviewInputChanged(this, null);
                    break;

                case _BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdPreviewLive:
                    if (PreviewLiveChanged != null)
                        PreviewLiveChanged(this, null);
                    break;

                case _BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdPreviewTransition:
                    if (PreviewTransitionChanged != null)
                        PreviewTransitionChanged(this, null);
                    break;

                case _BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdProgramInput:
                    if (ProgramInputChanged != null)
                        ProgramInputChanged(this, null);
                    break;

                case _BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdTransitionFramesRemaining:
                    if (TransitionFramesRemainingChanged != null)
                        TransitionFramesRemainingChanged(this, null);
                    break;

                case _BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdTransitionPosition:
                    if (TransitionPositionChanged != null)
                        TransitionPositionChanged(this, null);
                    break;
            }
        }

    }

    class SwitcherClipMonitor : IBMDSwitcherClipCallback
    {
        public event SwitcherEventHandler AudioValidChanged;
        public event SwitcherEventHandler HashChanged;
        public event SwitcherEventHandler LockBusy;
        public event SwitcherEventHandler LockIdle;
        public event SwitcherEventHandler NameChanged;
        public event SwitcherEventHandler TransferCancelled;
        public event SwitcherEventHandler TransferCompleted;
        public event SwitcherEventHandler TransferFailed;
        public event SwitcherEventHandler ValidChanged;

        public SwitcherClipMonitor()
        {
        }

        void IBMDSwitcherClipCallback.Notify(_BMDSwitcherMediaPoolEventType EventType, IBMDSwitcherFrame Frame, int FrameIndex, IBMDSwitcherAudio Audio, int ClipIndex)
        {
            switch (EventType)
            {
                case (_BMDSwitcherMediaPoolEventType.bmdSwitcherMediaPoolEventTypeAudioValidChanged):
                    if (AudioValidChanged != null)
                        AudioValidChanged(this, null);
                    break;

                case (_BMDSwitcherMediaPoolEventType.bmdSwitcherMediaPoolEventTypeHashChanged):
                    if (HashChanged != null)
                        HashChanged(this, null);
                    break;

                case (_BMDSwitcherMediaPoolEventType.bmdSwitcherMediaPoolEventTypeLockBusy):
                    if (LockBusy != null)
                        LockBusy(this, null);
                    break;

                case (_BMDSwitcherMediaPoolEventType.bmdSwitcherMediaPoolEventTypeLockIdle):
                    if (LockIdle != null)
                        LockIdle(this, null);
                    break;

                case (_BMDSwitcherMediaPoolEventType.bmdSwitcherMediaPoolEventTypeNameChanged):
                    if (NameChanged != null)
                        NameChanged(this, null);
                    break;

                case (_BMDSwitcherMediaPoolEventType.bmdSwitcherMediaPoolEventTypeTransferCancelled):
                    if (TransferCancelled != null)
                        TransferCancelled(this, null);
                    break;

                case (_BMDSwitcherMediaPoolEventType.bmdSwitcherMediaPoolEventTypeTransferCompleted):
                    if (TransferCompleted != null)
                        TransferCompleted(this, null);
                    break;

                case (_BMDSwitcherMediaPoolEventType.bmdSwitcherMediaPoolEventTypeTransferFailed):
                    if (TransferFailed != null)
                        TransferFailed(this, null);
                    break;

                case (_BMDSwitcherMediaPoolEventType.bmdSwitcherMediaPoolEventTypeValidChanged):
                    if (ValidChanged != null)
                        ValidChanged(this, null);
                    break;
            }
        }
    }

    class DownStreamKeyMonitor : IBMDSwitcherDownstreamKeyCallback
    {
        public event SwitcherEventHandler ClipChanged;
        public event SwitcherEventHandler FramesRemainingChanged;
        public event SwitcherEventHandler GainChanged;
        public event SwitcherEventHandler InputCutChanged;
        public event SwitcherEventHandler InputFillChanged;
        public event SwitcherEventHandler InverseChanged;
        public event SwitcherEventHandler IsAutoTransitioningChanged;
        public event SwitcherEventHandler IsTransitioningChanged;
        public event SwitcherEventHandler MaskBottomChanged;
        public event SwitcherEventHandler MaskedChanged;
        public event SwitcherEventHandler MaskLeftChanged;
        public event SwitcherEventHandler MaskRightChanged;
        public event SwitcherEventHandler MaskTopChanged;
        public event SwitcherEventHandler OnAirChanged;
        public event SwitcherEventHandler PreMultipliedChanged;
        public event SwitcherEventHandler RateChanged;
        public event SwitcherEventHandler TieChanged;

        public DownStreamKeyMonitor()
        {
        }

        void IBMDSwitcherDownstreamKeyCallback.Notify(_BMDSwitcherDownstreamKeyEventType EventType)
        {
            switch (EventType)
            {
                case (_BMDSwitcherDownstreamKeyEventType.bmdSwitcherDownstreamKeyEventTypeClipChanged):
                    if (ClipChanged != null)
                        ClipChanged(this, null);
                    break;

                case (_BMDSwitcherDownstreamKeyEventType.bmdSwitcherDownstreamKeyEventTypeFramesRemainingChanged):
                    if (FramesRemainingChanged != null)
                        FramesRemainingChanged(this, null);
                    break;

                case (_BMDSwitcherDownstreamKeyEventType.bmdSwitcherDownstreamKeyEventTypeGainChanged):
                    if (GainChanged != null)
                        GainChanged(this, null);
                    break;

                case (_BMDSwitcherDownstreamKeyEventType.bmdSwitcherDownstreamKeyEventTypeInputCutChanged):
                    if (InputCutChanged != null)
                        InputCutChanged(this, null);
                    break;

                case (_BMDSwitcherDownstreamKeyEventType.bmdSwitcherDownstreamKeyEventTypeInputFillChanged):
                    if (InputFillChanged != null)
                        InputFillChanged(this, null);
                    break;

                case (_BMDSwitcherDownstreamKeyEventType.bmdSwitcherDownstreamKeyEventTypeInverseChanged):
                    if (InverseChanged != null)
                        InverseChanged(this, null);
                    break;

                case (_BMDSwitcherDownstreamKeyEventType.bmdSwitcherDownstreamKeyEventTypeIsAutoTransitioningChanged):
                    if (IsAutoTransitioningChanged != null)
                        IsAutoTransitioningChanged(this, null);
                    break;

                case (_BMDSwitcherDownstreamKeyEventType.bmdSwitcherDownstreamKeyEventTypeIsTransitioningChanged):
                    if (IsTransitioningChanged != null)
                        IsTransitioningChanged(this, null);
                    break;

                case (_BMDSwitcherDownstreamKeyEventType.bmdSwitcherDownstreamKeyEventTypeMaskBottomChanged):
                    if (MaskBottomChanged != null)
                        MaskBottomChanged(this, null);
                    break;

                case (_BMDSwitcherDownstreamKeyEventType.bmdSwitcherDownstreamKeyEventTypeMaskedChanged):
                    if (MaskedChanged != null)
                        MaskedChanged(this, null);
                    break;

                case (_BMDSwitcherDownstreamKeyEventType.bmdSwitcherDownstreamKeyEventTypeMaskLeftChanged):
                    if (MaskLeftChanged != null)
                        MaskLeftChanged(this, null);
                    break;

                case (_BMDSwitcherDownstreamKeyEventType.bmdSwitcherDownstreamKeyEventTypeMaskRightChanged):
                    if (MaskRightChanged != null)
                        MaskRightChanged(this, null);
                    break;

                case (_BMDSwitcherDownstreamKeyEventType.bmdSwitcherDownstreamKeyEventTypeMaskTopChanged):
                    if (MaskTopChanged != null)
                        MaskTopChanged(this, null);
                    break;

                case (_BMDSwitcherDownstreamKeyEventType.bmdSwitcherDownstreamKeyEventTypeOnAirChanged):
                    if (OnAirChanged != null)
                        OnAirChanged(this, null);
                    break;

                case (_BMDSwitcherDownstreamKeyEventType.bmdSwitcherDownstreamKeyEventTypePreMultipliedChanged):
                    if (PreMultipliedChanged != null)
                        PreMultipliedChanged(this, null);
                    break;

                case (_BMDSwitcherDownstreamKeyEventType.bmdSwitcherDownstreamKeyEventTypeRateChanged):
                    if (RateChanged != null)
                        RateChanged(this, null);
                    break;

                case (_BMDSwitcherDownstreamKeyEventType.bmdSwitcherDownstreamKeyEventTypeTieChanged):
                    if (TieChanged != null)
                        TieChanged(this, null);
                    break;
            }
        }
    }

    class InputAuxMonitor : IBMDSwitcherInputAuxCallback
    {
        public event SwitcherEventHandler InputSourceChanged;

        public InputAuxMonitor()
        {
        }

        void IBMDSwitcherInputAuxCallback.Notify(_BMDSwitcherInputAuxEventType EventType)
        {
            switch (EventType)
            {
                case (_BMDSwitcherInputAuxEventType.bmdSwitcherInputAuxEventTypeInputSourceChanged):
                    if (InputSourceChanged != null)
                        InputSourceChanged(this, null);
                    break;
            }
        }
    }

    class InputColorMonitor : IBMDSwitcherInputColorCallback
    {
        public event SwitcherEventHandler HueChanged;
        public event SwitcherEventHandler LumaChanged;
        public event SwitcherEventHandler SaturationChanged;

        public InputColorMonitor()
        {
        }

        void IBMDSwitcherInputColorCallback.Notify(_BMDSwitcherInputColorEventType EventType)
        {
            switch (EventType)
            {
                case (_BMDSwitcherInputColorEventType.bmdSwitcherInputColorEventTypeHueChanged):
                    if (HueChanged != null)
                        HueChanged(this, null);
                    break;

                case (_BMDSwitcherInputColorEventType.bmdSwitcherInputColorEventTypeLumaChanged):
                    if (LumaChanged != null)
                        LumaChanged(this, null);
                    break;

                case (_BMDSwitcherInputColorEventType.bmdSwitcherInputColorEventTypeSaturationChanged):
                    if (SaturationChanged != null)
                        SaturationChanged(this, null);
                    break;
            }
        }
    }

    class InputSuperSourceMonitor : IBMDSwitcherInputSuperSourceCallback
    {
        public event SwitcherEventHandler ArtOptionChanged;
        public event SwitcherEventHandler BorderBevelChanged;
        public event SwitcherEventHandler BorderBevelPositionChanged;
        public event SwitcherEventHandler BorderBevelSoftnessChanged;
        public event SwitcherEventHandler BorderEnabledChanged;
        public event SwitcherEventHandler BorderHueChanged;
        public event SwitcherEventHandler BorderLightSourceAltitudeChanged;
        public event SwitcherEventHandler BorderLightSourceDirectionChanged;
        public event SwitcherEventHandler BorderLumaChanged;
        public event SwitcherEventHandler BorderSaturationChanged;
        public event SwitcherEventHandler BorderSoftnessInChanged;
        public event SwitcherEventHandler BorderSoftnessOutChanged;
        public event SwitcherEventHandler BorderWidthInChanged;
        public event SwitcherEventHandler BorderWidthOutChanged;
        public event SwitcherEventHandler ClipChanged;
        public event SwitcherEventHandler GainChanged;
        public event SwitcherEventHandler InputCutChanged;
        public event SwitcherEventHandler InputFillChanged;
        public event SwitcherEventHandler InverseChanged;
        public event SwitcherEventHandler PreMultipliedChanged;

        public InputSuperSourceMonitor()
        {
        }

        void IBMDSwitcherInputSuperSourceCallback.Notify(_BMDSwitcherInputSuperSourceEventType EventType)
        {
            switch (EventType)
            {
                case (_BMDSwitcherInputSuperSourceEventType.bmdSwitcherInputSuperSourceEventTypeArtOptionChanged):
                    if (ArtOptionChanged != null)
                        ArtOptionChanged(this, null);
                    break;

                case (_BMDSwitcherInputSuperSourceEventType.bmdSwitcherInputSuperSourceEventTypeBorderBevelChanged):
                    if (BorderBevelChanged != null)
                        BorderBevelChanged(this, null);
                    break;

                case (_BMDSwitcherInputSuperSourceEventType.bmdSwitcherInputSuperSourceEventTypeBorderBevelPositionChanged):
                    if (BorderBevelPositionChanged != null)
                        BorderBevelPositionChanged(this, null);
                    break;

                case (_BMDSwitcherInputSuperSourceEventType.bmdSwitcherInputSuperSourceEventTypeBorderBevelSoftnessChanged):
                    if (BorderBevelSoftnessChanged != null)
                        BorderBevelSoftnessChanged(this, null);
                    break;

                case (_BMDSwitcherInputSuperSourceEventType.bmdSwitcherInputSuperSourceEventTypeBorderEnabledChanged):
                    if (BorderEnabledChanged != null)
                        BorderEnabledChanged(this, null);
                    break;

                case (_BMDSwitcherInputSuperSourceEventType.bmdSwitcherInputSuperSourceEventTypeBorderHueChanged):
                    if (BorderHueChanged != null)
                        BorderHueChanged(this, null);
                    break;

                case (_BMDSwitcherInputSuperSourceEventType.bmdSwitcherInputSuperSourceEventTypeBorderLightSourceAltitudeChanged):
                    if (BorderLightSourceAltitudeChanged != null)
                        BorderLightSourceAltitudeChanged(this, null);
                    break;

                case (_BMDSwitcherInputSuperSourceEventType.bmdSwitcherInputSuperSourceEventTypeBorderLightSourceDirectionChanged):
                    if (BorderLightSourceDirectionChanged != null)
                        BorderLightSourceDirectionChanged(this, null);
                    break;

                case (_BMDSwitcherInputSuperSourceEventType.bmdSwitcherInputSuperSourceEventTypeBorderLumaChanged):
                    if (BorderLumaChanged != null)
                        BorderLumaChanged(this, null);
                    break;

                case (_BMDSwitcherInputSuperSourceEventType.bmdSwitcherInputSuperSourceEventTypeBorderSaturationChanged):
                    if (BorderSaturationChanged != null)
                        BorderSaturationChanged(this, null);
                    break;

                case (_BMDSwitcherInputSuperSourceEventType.bmdSwitcherInputSuperSourceEventTypeBorderSoftnessInChanged):
                    if (BorderSoftnessInChanged != null)
                        BorderSoftnessInChanged(this, null);
                    break;

                case (_BMDSwitcherInputSuperSourceEventType.bmdSwitcherInputSuperSourceEventTypeBorderSoftnessOutChanged):
                    if (BorderSoftnessOutChanged != null)
                        BorderSoftnessOutChanged(this, null);
                    break;

                case (_BMDSwitcherInputSuperSourceEventType.bmdSwitcherInputSuperSourceEventTypeBorderWidthInChanged):
                    if (BorderWidthInChanged != null)
                        BorderWidthInChanged(this, null);
                    break;

                case (_BMDSwitcherInputSuperSourceEventType.bmdSwitcherInputSuperSourceEventTypeBorderWidthOutChanged):
                    if (BorderWidthOutChanged != null)
                        BorderWidthOutChanged(this, null);
                    break;

                case (_BMDSwitcherInputSuperSourceEventType.bmdSwitcherInputSuperSourceEventTypeClipChanged):
                    if (ClipChanged != null)
                        ClipChanged(this, null);
                    break;

                case (_BMDSwitcherInputSuperSourceEventType.bmdSwitcherInputSuperSourceEventTypeGainChanged):
                    if (GainChanged != null)
                        GainChanged(this, null);
                    break;

                case (_BMDSwitcherInputSuperSourceEventType.bmdSwitcherInputSuperSourceEventTypeInputCutChanged):
                    if (InputCutChanged != null)
                        InputCutChanged(this, null);
                    break;

                case (_BMDSwitcherInputSuperSourceEventType.bmdSwitcherInputSuperSourceEventTypeInputFillChanged):
                    if (InputFillChanged != null)
                        InputFillChanged(this, null);
                    break;

                case (_BMDSwitcherInputSuperSourceEventType.bmdSwitcherInputSuperSourceEventTypeInverseChanged):
                    if (InverseChanged != null)
                        InverseChanged(this, null);
                    break;

                case (_BMDSwitcherInputSuperSourceEventType.bmdSwitcherInputSuperSourceEventTypePreMultipliedChanged):
                    if (PreMultipliedChanged != null)
                        PreMultipliedChanged(this, null);
                    break;
            }
        }
    }

    class KeyMonitor : IBMDSwitcherKeyCallback
    {
        public event SwitcherEventHandler CanBeDVEKeyChanged;
        public event SwitcherEventHandler InputCutChanged;
        public event SwitcherEventHandler InputFillChanged;
        public event SwitcherEventHandler MaskBottomChanged;
        public event SwitcherEventHandler MaskedChanged;
        public event SwitcherEventHandler MaskLeftChanged;
        public event SwitcherEventHandler MaskRightChanged;
        public event SwitcherEventHandler MaskTopChanged;
        public event SwitcherEventHandler OnAirChanged;
        public event SwitcherEventHandler TypeChanged;

        public KeyMonitor()
        {
        }

        void IBMDSwitcherKeyCallback.Notify(_BMDSwitcherKeyEventType EventType)
        {
            switch (EventType)
            {
                case (_BMDSwitcherKeyEventType.bmdSwitcherKeyEventTypeCanBeDVEKeyChanged):
                    if (CanBeDVEKeyChanged != null)
                        CanBeDVEKeyChanged(this, null);
                    break;

                case (_BMDSwitcherKeyEventType.bmdSwitcherKeyEventTypeInputCutChanged):
                    if (InputCutChanged != null)
                        InputCutChanged(this, null);
                    break;

                case (_BMDSwitcherKeyEventType.bmdSwitcherKeyEventTypeInputFillChanged):
                    if (InputFillChanged != null)
                        InputFillChanged(this, null);
                    break;

                case (_BMDSwitcherKeyEventType.bmdSwitcherKeyEventTypeMaskBottomChanged):
                    if (MaskBottomChanged != null)
                        MaskBottomChanged(this, null);
                    break;

                case (_BMDSwitcherKeyEventType.bmdSwitcherKeyEventTypeMaskedChanged):
                    if (MaskedChanged != null)
                        MaskedChanged(this, null);
                    break;

                case (_BMDSwitcherKeyEventType.bmdSwitcherKeyEventTypeMaskLeftChanged):
                    if (MaskLeftChanged != null)
                        MaskLeftChanged(this, null);
                    break;

                case (_BMDSwitcherKeyEventType.bmdSwitcherKeyEventTypeMaskRightChanged):
                    if (MaskRightChanged != null)
                        MaskRightChanged(this, null);
                    break;

                case (_BMDSwitcherKeyEventType.bmdSwitcherKeyEventTypeMaskTopChanged):
                    if (MaskTopChanged != null)
                        MaskTopChanged(this, null);
                    break;

                case (_BMDSwitcherKeyEventType.bmdSwitcherKeyEventTypeOnAirChanged):
                    if (OnAirChanged != null)
                        OnAirChanged(this, null);
                    break;

                case (_BMDSwitcherKeyEventType.bmdSwitcherKeyEventTypeTypeChanged):
                    if (TypeChanged != null)
                        TypeChanged(this, null);
                    break;
            }
        }
    }

    class ChromaKeyMonitor : IBMDSwitcherKeyChromaParametersCallback
    {
        public event SwitcherEventHandler GainChanged;
        public event SwitcherEventHandler HueChanged;
        public event SwitcherEventHandler LiftChanged;
        public event SwitcherEventHandler NarrowChanged;
        public event SwitcherEventHandler YSuppressChanged;

        public ChromaKeyMonitor()
        {
        }

        void IBMDSwitcherKeyChromaParametersCallback.Notify(_BMDSwitcherKeyChromaParametersEventType EventType)
        {
            switch (EventType)
            {
                case (_BMDSwitcherKeyChromaParametersEventType.bmdSwitcherKeyChromaParametersEventTypeGainChanged):
                    if (GainChanged != null)
                        GainChanged(this, null);
                    break;

                case (_BMDSwitcherKeyChromaParametersEventType.bmdSwitcherKeyChromaParametersEventTypeHueChanged):
                    if (HueChanged != null)
                        HueChanged(this, null);
                    break;

                case (_BMDSwitcherKeyChromaParametersEventType.bmdSwitcherKeyChromaParametersEventTypeLiftChanged):
                    if (LiftChanged != null)
                        LiftChanged(this, null);
                    break;

                case (_BMDSwitcherKeyChromaParametersEventType.bmdSwitcherKeyChromaParametersEventTypeNarrowChanged):
                    if (NarrowChanged != null)
                        NarrowChanged(this, null);
                    break;

                case (_BMDSwitcherKeyChromaParametersEventType.bmdSwitcherKeyChromaParametersEventTypeYSuppressChanged):
                    if (YSuppressChanged != null)
                        YSuppressChanged(this, null);
                    break;
            }
        }
    }

    class DVEKeyMonitor : IBMDSwitcherKeyDVEParametersCallback
    {
        public event SwitcherEventHandler BorderBevelChanged;
        public event SwitcherEventHandler BorderBevelPositionChanged;
        public event SwitcherEventHandler BorderBevelSoftnessChanged;
        public event SwitcherEventHandler BorderEnabledChanged;
        public event SwitcherEventHandler BorderHueChanged;
        public event SwitcherEventHandler BorderLumaChanged;
        public event SwitcherEventHandler BorderOpacityChanged;
        public event SwitcherEventHandler BorderSaturationChanged;
        public event SwitcherEventHandler BorderSoftnessInChanged;
        public event SwitcherEventHandler BorderSoftnessOutChanged;
        public event SwitcherEventHandler BorderWidthInChanged;
        public event SwitcherEventHandler BorderWidthOutChanged;
        public event SwitcherEventHandler LightSourceAltitudeChanged;
        public event SwitcherEventHandler LightSourceDirectionChanged;
        public event SwitcherEventHandler MaskBottomChanged;
        public event SwitcherEventHandler MaskedChanged;
        public event SwitcherEventHandler MaskLeftChanged;
        public event SwitcherEventHandler MaskRightChanged;
        public event SwitcherEventHandler MaskTopChanged;
        public event SwitcherEventHandler ShadowChanged;

        public DVEKeyMonitor()
        {
        }

        void IBMDSwitcherKeyDVEParametersCallback.Notify(_BMDSwitcherKeyDVEParametersEventType EventType)
        {
            switch (EventType)
            {
                case (_BMDSwitcherKeyDVEParametersEventType.bmdSwitcherKeyDVEParametersEventTypeBorderBevelChanged):
                    if (BorderBevelChanged != null)
                        BorderBevelChanged(this, null);
                    break;

                case (_BMDSwitcherKeyDVEParametersEventType.bmdSwitcherKeyDVEParametersEventTypeBorderBevelPositionChanged):
                    if (BorderBevelPositionChanged != null)
                        BorderBevelPositionChanged(this, null);
                    break;

                case (_BMDSwitcherKeyDVEParametersEventType.bmdSwitcherKeyDVEParametersEventTypeBorderBevelSoftnessChanged):
                    if (BorderBevelSoftnessChanged != null)
                        BorderBevelSoftnessChanged(this, null);
                    break;

                case (_BMDSwitcherKeyDVEParametersEventType.bmdSwitcherKeyDVEParametersEventTypeBorderEnabledChanged):
                    if (BorderEnabledChanged != null)
                        BorderEnabledChanged(this, null);
                    break;

                case (_BMDSwitcherKeyDVEParametersEventType.bmdSwitcherKeyDVEParametersEventTypeBorderHueChanged):
                    if (BorderHueChanged != null)
                        BorderHueChanged(this, null);
                    break;

                case (_BMDSwitcherKeyDVEParametersEventType.bmdSwitcherKeyDVEParametersEventTypeBorderLumaChanged):
                    if (BorderLumaChanged != null)
                        BorderLumaChanged(this, null);
                    break;

                case (_BMDSwitcherKeyDVEParametersEventType.bmdSwitcherKeyDVEParametersEventTypeBorderOpacityChanged):
                    if (BorderOpacityChanged != null)
                        BorderOpacityChanged(this, null);
                    break;

                case (_BMDSwitcherKeyDVEParametersEventType.bmdSwitcherKeyDVEParametersEventTypeBorderSaturationChanged):
                    if (BorderSaturationChanged != null)
                        BorderSaturationChanged(this, null);
                    break;

                case (_BMDSwitcherKeyDVEParametersEventType.bmdSwitcherKeyDVEParametersEventTypeBorderSoftnessInChanged):
                    if (BorderSoftnessInChanged != null)
                        BorderSoftnessInChanged(this, null);
                    break;

                case (_BMDSwitcherKeyDVEParametersEventType.bmdSwitcherKeyDVEParametersEventTypeBorderSoftnessOutChanged):
                    if (BorderSoftnessOutChanged != null)
                        BorderSoftnessOutChanged(this, null);
                    break;

                case (_BMDSwitcherKeyDVEParametersEventType.bmdSwitcherKeyDVEParametersEventTypeBorderWidthInChanged):
                    if (BorderWidthInChanged != null)
                        BorderWidthInChanged(this, null);
                    break;

                case (_BMDSwitcherKeyDVEParametersEventType.bmdSwitcherKeyDVEParametersEventTypeBorderWidthOutChanged):
                    if (BorderWidthOutChanged != null)
                        BorderWidthOutChanged(this, null);
                    break;

                case (_BMDSwitcherKeyDVEParametersEventType.bmdSwitcherKeyDVEParametersEventTypeLightSourceAltitudeChanged):
                    if (LightSourceAltitudeChanged != null)
                        LightSourceAltitudeChanged(this, null);
                    break;

                case (_BMDSwitcherKeyDVEParametersEventType.bmdSwitcherKeyDVEParametersEventTypeLightSourceDirectionChanged):
                    if (LightSourceDirectionChanged != null)
                        LightSourceDirectionChanged(this, null);
                    break;

                case (_BMDSwitcherKeyDVEParametersEventType.bmdSwitcherKeyDVEParametersEventTypeMaskBottomChanged):
                    if (MaskBottomChanged != null)
                        MaskBottomChanged(this, null);
                    break;

                case (_BMDSwitcherKeyDVEParametersEventType.bmdSwitcherKeyDVEParametersEventTypeMaskedChanged):
                    if (MaskedChanged != null)
                        MaskedChanged(this, null);
                    break;

                case (_BMDSwitcherKeyDVEParametersEventType.bmdSwitcherKeyDVEParametersEventTypeMaskLeftChanged):
                    if (MaskLeftChanged != null)
                        MaskLeftChanged(this, null);
                    break;

                case (_BMDSwitcherKeyDVEParametersEventType.bmdSwitcherKeyDVEParametersEventTypeMaskRightChanged):
                    if (MaskRightChanged != null)
                        MaskRightChanged(this, null);
                    break;

                case (_BMDSwitcherKeyDVEParametersEventType.bmdSwitcherKeyDVEParametersEventTypeMaskTopChanged):
                    if (MaskTopChanged != null)
                        MaskTopChanged(this, null);
                    break;

                case (_BMDSwitcherKeyDVEParametersEventType.bmdSwitcherKeyDVEParametersEventTypeShadowChanged):
                    if (ShadowChanged != null)
                        ShadowChanged(this, null);
                    break;
            }
        }
    }

    class FlyKeyMonitor : IBMDSwitcherKeyFlyParametersCallback
    {
        public event SwitcherEventHandler CanFlyChanged;
        public event SwitcherEventHandler FlyChanged;
        public event SwitcherEventHandler IsAtKeyFramesChanged;
        public event SwitcherEventHandler IsKeyFrameStoredChanged;
        public event SwitcherEventHandler IsRunningChanged;
        public event SwitcherEventHandler PositionXChanged;
        public event SwitcherEventHandler PositionYChanged;
        public event SwitcherEventHandler RateChanged;
        public event SwitcherEventHandler RotationChanged;
        public event SwitcherEventHandler SizeXChanged;
        public event SwitcherEventHandler SizeYChanged;

        public FlyKeyMonitor()
        {
        }

        void IBMDSwitcherKeyFlyParametersCallback.Notify(_BMDSwitcherKeyFlyParametersEventType EventType, _BMDSwitcherFlyKeyFrame KeyFrame)
        {
            switch (EventType)
            {
                case (_BMDSwitcherKeyFlyParametersEventType.bmdSwitcherKeyFlyParametersEventTypeCanFlyChanged):
                    if (CanFlyChanged != null)
                        CanFlyChanged(this, null);
                    break;

                case (_BMDSwitcherKeyFlyParametersEventType.bmdSwitcherKeyFlyParametersEventTypeFlyChanged):
                    if (FlyChanged != null)
                        FlyChanged(this, null);
                    break;

                case (_BMDSwitcherKeyFlyParametersEventType.bmdSwitcherKeyFlyParametersEventTypeIsAtKeyFramesChanged):
                    if (IsAtKeyFramesChanged != null)
                        IsAtKeyFramesChanged(this, null);
                    break;

                case (_BMDSwitcherKeyFlyParametersEventType.bmdSwitcherKeyFlyParametersEventTypeIsKeyFrameStoredChanged):
                    if (IsKeyFrameStoredChanged != null)
                        IsKeyFrameStoredChanged(this, null);
                    break;

                case (_BMDSwitcherKeyFlyParametersEventType.bmdSwitcherKeyFlyParametersEventTypeIsRunningChanged):
                    if (IsRunningChanged != null)
                        IsRunningChanged(this, null);
                    break;

                case (_BMDSwitcherKeyFlyParametersEventType.bmdSwitcherKeyFlyParametersEventTypePositionXChanged):
                    if (PositionXChanged != null)
                        PositionXChanged(this, null);
                    break;

                case (_BMDSwitcherKeyFlyParametersEventType.bmdSwitcherKeyFlyParametersEventTypePositionYChanged):
                    if (PositionYChanged != null)
                        PositionYChanged(this, null);
                    break;

                case (_BMDSwitcherKeyFlyParametersEventType.bmdSwitcherKeyFlyParametersEventTypeRateChanged):
                    if (RateChanged != null)
                        RateChanged(this, null);
                    break;

                case (_BMDSwitcherKeyFlyParametersEventType.bmdSwitcherKeyFlyParametersEventTypeRotationChanged):
                    if (RotationChanged != null)
                        RotationChanged(this, null);
                    break;

                case (_BMDSwitcherKeyFlyParametersEventType.bmdSwitcherKeyFlyParametersEventTypeSizeXChanged):
                    if (SizeXChanged != null)
                        SizeXChanged(this, null);
                    break;

                case (_BMDSwitcherKeyFlyParametersEventType.bmdSwitcherKeyFlyParametersEventTypeSizeYChanged):
                    if (SizeYChanged != null)
                        SizeYChanged(this, null);
                    break;
            }
        }
    }

    class LumaKeyMonitor : IBMDSwitcherKeyLumaParametersCallback
    {
        public event SwitcherEventHandler ClipChanged;
        public event SwitcherEventHandler GainChanged;
        public event SwitcherEventHandler InverseChanged;
        public event SwitcherEventHandler PreMultipliedChanged;

        public LumaKeyMonitor()
        {
        }

        void IBMDSwitcherKeyLumaParametersCallback.Notify(_BMDSwitcherKeyLumaParametersEventType EventType)
        {
            switch (EventType)
            {
                case (_BMDSwitcherKeyLumaParametersEventType.bmdSwitcherKeyLumaParametersEventTypeClipChanged):
                    if (ClipChanged != null)
                        ClipChanged(this, null);
                    break;

                case (_BMDSwitcherKeyLumaParametersEventType.bmdSwitcherKeyLumaParametersEventTypeGainChanged):
                    if (GainChanged != null)
                        GainChanged(this, null);
                    break;

                case (_BMDSwitcherKeyLumaParametersEventType.bmdSwitcherKeyLumaParametersEventTypeInverseChanged):
                    if (InverseChanged != null)
                        InverseChanged(this, null);
                    break;

                case (_BMDSwitcherKeyLumaParametersEventType.bmdSwitcherKeyLumaParametersEventTypePreMultipliedChanged):
                    if (PreMultipliedChanged != null)
                        PreMultipliedChanged(this, null);
                    break;
            }
        }
    }

    class PatternKeyMonitor : IBMDSwitcherKeyPatternParametersCallback
    {
        public event SwitcherEventHandler HorizontalOffestChanged;
        public event SwitcherEventHandler InverseChanged;
        public event SwitcherEventHandler PatternChanged;
        public event SwitcherEventHandler SizeChanged;
        public event SwitcherEventHandler SoftnessChanged;
        public event SwitcherEventHandler SymmetryChanged;
        public event SwitcherEventHandler VerticalOffsetChanged;

        public PatternKeyMonitor()
        {
        }

        void IBMDSwitcherKeyPatternParametersCallback.Notify(_BMDSwitcherKeyPatternParametersEventType EventType)
        {
            switch (EventType)
            {
                case (_BMDSwitcherKeyPatternParametersEventType.bmdSwitcherKeyPatternParametersEventTypeHorizontalOffsetChanged):
                    if (HorizontalOffestChanged != null)
                        HorizontalOffestChanged(this, null);
                    break;

                case (_BMDSwitcherKeyPatternParametersEventType.bmdSwitcherKeyPatternParametersEventTypeInverseChanged):
                    if (InverseChanged != null)
                        InverseChanged(this, null);
                    break;

                case (_BMDSwitcherKeyPatternParametersEventType.bmdSwitcherKeyPatternParametersEventTypePatternChanged):
                    if (PatternChanged != null)
                        PatternChanged(this, null);
                    break;

                case (_BMDSwitcherKeyPatternParametersEventType.bmdSwitcherKeyPatternParametersEventTypeSizeChanged):
                    if (SizeChanged != null)
                        SizeChanged(this, null);
                    break;

                case (_BMDSwitcherKeyPatternParametersEventType.bmdSwitcherKeyPatternParametersEventTypeSoftnessChanged):
                    if (SoftnessChanged != null)
                        SoftnessChanged(this, null);
                    break;

                case (_BMDSwitcherKeyPatternParametersEventType.bmdSwitcherKeyPatternParametersEventTypeSymmetryChanged):
                    if (SymmetryChanged != null)
                        SymmetryChanged(this, null);
                    break;

                case (_BMDSwitcherKeyPatternParametersEventType.bmdSwitcherKeyPatternParametersEventTypeVerticalOffsetChanged):
                    if (VerticalOffsetChanged != null)
                        VerticalOffsetChanged(this, null);
                    break;
            }
        }
    }

    class LockMonitor : IBMDSwitcherLockCallback
    {
        public event SwitcherEventHandler Obtained;

        public LockMonitor()
        {
        }

        void IBMDSwitcherLockCallback.Obtained()
        {
            if (Obtained != null)
                Obtained(this, null);
        }
    }

    class MediaPlayerMonitor : IBMDSwitcherMediaPlayerCallback
    {
        public event SwitcherEventHandler AtBeginningChanged;
        public event SwitcherEventHandler ClipFrameChanged;
        public event SwitcherEventHandler LoopChanged;
        public event SwitcherEventHandler PlayingChanged;
        public event SwitcherEventHandler SourceChanged;

        public MediaPlayerMonitor()
        {
        }

        void IBMDSwitcherMediaPlayerCallback.AtBeginningChanged()
        {
            if (AtBeginningChanged != null)
                AtBeginningChanged(this, null);
        }

        void IBMDSwitcherMediaPlayerCallback.ClipFrameChanged()
        {
            if (ClipFrameChanged != null)
                ClipFrameChanged(this, null);
        }

        void IBMDSwitcherMediaPlayerCallback.LoopChanged()
        {
            if (LoopChanged != null)
                LoopChanged(this, null);
        }

        void IBMDSwitcherMediaPlayerCallback.PlayingChanged()
        {
            if (PlayingChanged != null)
                PlayingChanged(this, null);
        }

        void IBMDSwitcherMediaPlayerCallback.SourceChanged()
        {
            if (SourceChanged != null)
                SourceChanged(this, null);
        }
    }

    class MediaPoolMonitor : IBMDSwitcherMediaPoolCallback
    {
        public event SwitcherEventHandler ClipFrameMaxCountsChanged;
        public event SwitcherEventHandler FrameTotalForClipsChanged;

        public MediaPoolMonitor()
        {
        }

        void IBMDSwitcherMediaPoolCallback.ClipFrameMaxCountsChanged()
        {
            if (ClipFrameMaxCountsChanged != null)
                ClipFrameMaxCountsChanged(this, null);
        }

        void IBMDSwitcherMediaPoolCallback.FrameTotalForClipsChanged()
        {
            if (FrameTotalForClipsChanged != null)
                FrameTotalForClipsChanged(this, null);
        }
    }

    class MultiviewMonitor : IBMDSwitcherMultiViewCallback
    {
        public event SwitcherEventHandler LayoutChanged;
        public event SwitcherEventHandler WindowChanged;

        public MultiviewMonitor()
        {
        }

        void IBMDSwitcherMultiViewCallback.Notify(_BMDSwitcherMultiViewEventType EventType, int Window)
        {
            switch (EventType)
            {
                case (_BMDSwitcherMultiViewEventType.bmdSwitcherMultiViewEventTypeLayoutChanged):
                    if (LayoutChanged != null)
                        LayoutChanged(this, null);
                    break;

                case (_BMDSwitcherMultiViewEventType.bmdSwitcherMultiViewEventTypeWindowChanged):
                    if (WindowChanged != null)
                        WindowChanged(this, null);
                    break;
            }
        }
    }

    class StillsMonitor : IBMDSwitcherStillsCallback
    {
        public event SwitcherEventHandler AudioValidChanged;
        public event SwitcherEventHandler HashChanged;
        public event SwitcherEventHandler LockBusy;
        public event SwitcherEventHandler LockIdle;
        public event SwitcherEventHandler NameChanged;
        public event SwitcherEventHandler TransferCancelled;
        public event SwitcherEventHandler TransferCompleted;
        public event SwitcherEventHandler TransferFailed;
        public event SwitcherEventHandler ValidChanged;

        public StillsMonitor()
        {
        }

        void IBMDSwitcherStillsCallback.Notify(_BMDSwitcherMediaPoolEventType EventType, IBMDSwitcherFrame Frame, int index)
        {
            switch (EventType)
            {
                case (_BMDSwitcherMediaPoolEventType.bmdSwitcherMediaPoolEventTypeAudioValidChanged):
                    if (AudioValidChanged != null)
                        AudioValidChanged(this, null);
                    break;

                case (_BMDSwitcherMediaPoolEventType.bmdSwitcherMediaPoolEventTypeHashChanged):
                    if (HashChanged != null)
                        HashChanged(this, null);
                    break;

                case (_BMDSwitcherMediaPoolEventType.bmdSwitcherMediaPoolEventTypeLockBusy):
                    if (LockBusy != null)
                        LockBusy(this, null);
                    break;

                case (_BMDSwitcherMediaPoolEventType.bmdSwitcherMediaPoolEventTypeLockIdle):
                    if (LockIdle != null)
                        LockIdle(this, null);
                    break;

                case (_BMDSwitcherMediaPoolEventType.bmdSwitcherMediaPoolEventTypeNameChanged):
                    if (NameChanged != null)
                        NameChanged(this, null);
                    break;

                case (_BMDSwitcherMediaPoolEventType.bmdSwitcherMediaPoolEventTypeTransferCancelled):
                    if (TransferCancelled != null)
                        TransferCancelled(this, null);
                    break;

                case (_BMDSwitcherMediaPoolEventType.bmdSwitcherMediaPoolEventTypeTransferCompleted):
                    if (TransferCompleted != null)
                        TransferCompleted(this, null);
                    break;

                case (_BMDSwitcherMediaPoolEventType.bmdSwitcherMediaPoolEventTypeTransferFailed):
                    if (TransferFailed != null)
                        TransferFailed(this, null);
                    break;

                case (_BMDSwitcherMediaPoolEventType.bmdSwitcherMediaPoolEventTypeValidChanged):
                    if (ValidChanged != null)
                        ValidChanged(this, null);
                    break;
            }
        }
    }

    class TransitionMonitor : IBMDSwitcherTransitionParametersCallback
    {
        public event SwitcherEventHandler NextTransitionSelectionChanged;
        public event SwitcherEventHandler NextTransitionStyleChanged;
        public event SwitcherEventHandler TransitionSelectionChanged;
        public event SwitcherEventHandler TransitionStyleChanged;

        public TransitionMonitor()
        {
        }

        void IBMDSwitcherTransitionParametersCallback.Notify(_BMDSwitcherTransitionParametersEventType EventType)
        {
            switch (EventType)
            {
                case (_BMDSwitcherTransitionParametersEventType.bmdSwitcherTransitionParametersEventTypeNextTransitionSelectionChanged):
                    if (NextTransitionSelectionChanged != null)
                        NextTransitionSelectionChanged(this, null);
                    break;

                case (_BMDSwitcherTransitionParametersEventType.bmdSwitcherTransitionParametersEventTypeNextTransitionStyleChanged):
                    if (NextTransitionStyleChanged != null)
                        NextTransitionStyleChanged(this, null);
                    break;

                case (_BMDSwitcherTransitionParametersEventType.bmdSwitcherTransitionParametersEventTypeTransitionSelectionChanged):
                    if (TransitionSelectionChanged != null)
                        TransitionSelectionChanged(this, null);
                    break;

                case (_BMDSwitcherTransitionParametersEventType.bmdSwitcherTransitionParametersEventTypeTransitionStyleChanged):
                    if (TransitionStyleChanged != null)
                        TransitionStyleChanged(this, null);
                    break;
            }
        }
    }

    class DipTransitionMonitor : IBMDSwitcherTransitionDipParametersCallback
    {
        public event SwitcherEventHandler InputDipChanged;
        public event SwitcherEventHandler RateChanged;

        public DipTransitionMonitor()
        {
        }

        void IBMDSwitcherTransitionDipParametersCallback.Notify(_BMDSwitcherTransitionDipParametersEventType EventType)
        {
            switch (EventType)
            {
                case (_BMDSwitcherTransitionDipParametersEventType.bmdSwitcherTransitionDipParametersEventTypeInputDipChanged):
                    if (InputDipChanged != null)
                        InputDipChanged(this, null);
                    break;

                case (_BMDSwitcherTransitionDipParametersEventType.bmdSwitcherTransitionDipParametersEventTypeRateChanged):
                    if (RateChanged != null)
                        RateChanged(this, null);
                    break;
            }
        }
    }

    class DVETransitionMonitor : IBMDSwitcherTransitionDVEParametersCallback
    {
        public event SwitcherEventHandler ClipChanged;
        public event SwitcherEventHandler EnableKeyChanged;
        public event SwitcherEventHandler FlipFlopChanged;
        public event SwitcherEventHandler GainChanged;
        public event SwitcherEventHandler InputCutChanged;
        public event SwitcherEventHandler InputFillChanged;
        public event SwitcherEventHandler InverseChanged;
        public event SwitcherEventHandler LogoRateChanged;
        public event SwitcherEventHandler PreMultipliedChanged;
        public event SwitcherEventHandler RateChanged;
        public event SwitcherEventHandler ReverseChanged;
        public event SwitcherEventHandler StyleChanged;

        public DVETransitionMonitor()
        {
        }

        void IBMDSwitcherTransitionDVEParametersCallback.Notify(_BMDSwitcherTransitionDVEParametersEventType EventType)
        {
            switch (EventType)
            {
                case (_BMDSwitcherTransitionDVEParametersEventType.bmdSwitcherTransitionDVEParametersEventTypeClipChanged):
                    if (ClipChanged != null)
                        ClipChanged(this, null);
                    break;

                case (_BMDSwitcherTransitionDVEParametersEventType.bmdSwitcherTransitionDVEParametersEventTypeEnableKeyChanged):
                    if (EnableKeyChanged != null)
                        EnableKeyChanged(this, null);
                    break;

                case (_BMDSwitcherTransitionDVEParametersEventType.bmdSwitcherTransitionDVEParametersEventTypeFlipFlopChanged):
                    if (FlipFlopChanged != null)
                        FlipFlopChanged(this, null);
                    break;

                case (_BMDSwitcherTransitionDVEParametersEventType.bmdSwitcherTransitionDVEParametersEventTypeGainChanged):
                    if (GainChanged != null)
                        GainChanged(this, null);
                    break;

                case (_BMDSwitcherTransitionDVEParametersEventType.bmdSwitcherTransitionDVEParametersEventTypeInputCutChanged):
                    if (InputCutChanged != null)
                        InputCutChanged(this, null);
                    break;

                case (_BMDSwitcherTransitionDVEParametersEventType.bmdSwitcherTransitionDVEParametersEventTypeInputFillChanged):
                    if (InputFillChanged != null)
                        InputFillChanged(this, null);
                    break;

                case (_BMDSwitcherTransitionDVEParametersEventType.bmdSwitcherTransitionDVEParametersEventTypeInverseChanged):
                    if (InverseChanged != null)
                        InverseChanged(this, null);
                    break;

                case (_BMDSwitcherTransitionDVEParametersEventType.bmdSwitcherTransitionDVEParametersEventTypeLogoRateChanged):
                    if (LogoRateChanged != null)
                        LogoRateChanged(this, null);
                    break;

                case (_BMDSwitcherTransitionDVEParametersEventType.bmdSwitcherTransitionDVEParametersEventTypePreMultipliedChanged):
                    if (PreMultipliedChanged != null)
                        PreMultipliedChanged(this, null);
                    break;

                case (_BMDSwitcherTransitionDVEParametersEventType.bmdSwitcherTransitionDVEParametersEventTypeRateChanged):
                    if (RateChanged != null)
                        RateChanged(this, null);
                    break;

                case (_BMDSwitcherTransitionDVEParametersEventType.bmdSwitcherTransitionDVEParametersEventTypeReverseChanged):
                    if (ReverseChanged != null)
                        ReverseChanged(this, null);
                    break;

                case (_BMDSwitcherTransitionDVEParametersEventType.bmdSwitcherTransitionDVEParametersEventTypeStyleChanged):
                    if (StyleChanged != null)
                        StyleChanged(this, null);
                    break;
            }
        }
    }

    class MixTransitionMonitor : IBMDSwitcherTransitionMixParametersCallback
    {
        public event SwitcherEventHandler RateChanged;

        public MixTransitionMonitor()
        {
        }

        void IBMDSwitcherTransitionMixParametersCallback.Notify(_BMDSwitcherTransitionMixParametersEventType EventType)
        {
            switch (EventType)
            {
                case (_BMDSwitcherTransitionMixParametersEventType.bmdSwitcherTransitionMixParametersEventTypeRateChanged):
                    if (RateChanged != null)
                        RateChanged(this, null);
                    break;
            }
        }
    }

    class StingerTransitionMonitor : IBMDSwitcherTransitionStingerParametersCallback
    {
        public event SwitcherEventHandler ClipChanged;
        public event SwitcherEventHandler ClipDurationChanged;
        public event SwitcherEventHandler GainChanged;
        public event SwitcherEventHandler InverseChanged;
        public event SwitcherEventHandler MixRateChanged;
        public event SwitcherEventHandler PreMultipliedChanged;
        public event SwitcherEventHandler PrerollChanged;
        public event SwitcherEventHandler SourceChanged;
        public event SwitcherEventHandler TriggerPointChanged;

        public StingerTransitionMonitor()
        {
        }

        void IBMDSwitcherTransitionStingerParametersCallback.Notify(_BMDSwitcherTransitionStingerParametersEventType EventType)
        {
            switch (EventType)
            {
                case (_BMDSwitcherTransitionStingerParametersEventType.bmdSwitcherTransitionStingerParametersEventTypeClipChanged):
                    if (ClipChanged != null)
                        ClipChanged(this, null);
                    break;

                case (_BMDSwitcherTransitionStingerParametersEventType.bmdSwitcherTransitionStingerParametersEventTypeClipDurationChanged):
                    if (ClipDurationChanged != null)
                        ClipDurationChanged(this, null);
                    break;

                case (_BMDSwitcherTransitionStingerParametersEventType.bmdSwitcherTransitionStingerParametersEventTypeGainChanged):
                    if (GainChanged != null)
                        GainChanged(this, null);
                    break;

                case (_BMDSwitcherTransitionStingerParametersEventType.bmdSwitcherTransitionStingerParametersEventTypeInverseChanged):
                    if (InverseChanged != null)
                        InverseChanged(this, null);
                    break;

                case (_BMDSwitcherTransitionStingerParametersEventType.bmdSwitcherTransitionStingerParametersEventTypeMixRateChanged):
                    if (MixRateChanged != null)
                        MixRateChanged(this, null);
                    break;

                case (_BMDSwitcherTransitionStingerParametersEventType.bmdSwitcherTransitionStingerParametersEventTypePreMultipliedChanged):
                    if (PreMultipliedChanged != null)
                        PreMultipliedChanged(this, null);
                    break;

                case (_BMDSwitcherTransitionStingerParametersEventType.bmdSwitcherTransitionStingerParametersEventTypePrerollChanged):
                    if (PrerollChanged != null)
                        PrerollChanged(this, null);
                    break;

                case (_BMDSwitcherTransitionStingerParametersEventType.bmdSwitcherTransitionStingerParametersEventTypeSourceChanged):
                    if (SourceChanged != null)
                        SourceChanged(this, null);
                    break;

                case (_BMDSwitcherTransitionStingerParametersEventType.bmdSwitcherTransitionStingerParametersEventTypeTriggerPointChanged):
                    if (TriggerPointChanged != null)
                        TriggerPointChanged(this, null);
                    break;
            }
        }
    }

    class WipeTransitionMonitor : IBMDSwitcherTransitionWipeParametersCallback
    {
        public event SwitcherEventHandler BorderSizeChanged;
        public event SwitcherEventHandler FlipFlopChanged;
        public event SwitcherEventHandler HorizontalOffestChanged;
        public event SwitcherEventHandler InputBorderChanged;
        public event SwitcherEventHandler PatternChanged;
        public event SwitcherEventHandler RateChanged;
        public event SwitcherEventHandler ReverseChanged;
        public event SwitcherEventHandler SoftnessChanged;
        public event SwitcherEventHandler SymmetryChanged;
        public event SwitcherEventHandler VerticalOffsetChanged;

        public WipeTransitionMonitor()
        {
        }

        void IBMDSwitcherTransitionWipeParametersCallback.Notify(_BMDSwitcherTransitionWipeParametersEventType EventType)
        {
            switch (EventType)
            {
                case (_BMDSwitcherTransitionWipeParametersEventType.bmdSwitcherTransitionWipeParametersEventTypeBorderSizeChanged):
                    if (BorderSizeChanged != null)
                        BorderSizeChanged(this, null);
                    break;

                case (_BMDSwitcherTransitionWipeParametersEventType.bmdSwitcherTransitionWipeParametersEventTypeFlipFlopChanged):
                    if (FlipFlopChanged != null)
                        FlipFlopChanged(this, null);
                    break;

                case (_BMDSwitcherTransitionWipeParametersEventType.bmdSwitcherTransitionWipeParametersEventTypeHorizontalOffsetChanged):
                    if (HorizontalOffestChanged != null)
                        HorizontalOffestChanged(this, null);
                    break;

                case (_BMDSwitcherTransitionWipeParametersEventType.bmdSwitcherTransitionWipeParametersEventTypeInputBorderChanged):
                    if (InputBorderChanged != null)
                        InputBorderChanged(this, null);
                    break;

                case (_BMDSwitcherTransitionWipeParametersEventType.bmdSwitcherTransitionWipeParametersEventTypePatternChanged):
                    if (PatternChanged != null)
                        PatternChanged(this, null);
                    break;

                case (_BMDSwitcherTransitionWipeParametersEventType.bmdSwitcherTransitionWipeParametersEventTypeRateChanged):
                    if (RateChanged != null)
                        RateChanged(this, null);
                    break;

                case (_BMDSwitcherTransitionWipeParametersEventType.bmdSwitcherTransitionWipeParametersEventTypeReverseChanged):
                    if (ReverseChanged != null)
                        ReverseChanged(this, null);
                    break;

                case (_BMDSwitcherTransitionWipeParametersEventType.bmdSwitcherTransitionWipeParametersEventTypeSoftnessChanged):
                    if (SoftnessChanged != null)
                        SoftnessChanged(this, null);
                    break;

                case (_BMDSwitcherTransitionWipeParametersEventType.bmdSwitcherTransitionWipeParametersEventTypeSymmetryChanged):
                    if (SymmetryChanged != null)
                        SymmetryChanged(this, null);
                    break;

                case (_BMDSwitcherTransitionWipeParametersEventType.bmdSwitcherTransitionWipeParametersEventTypeVerticalOffsetChanged):
                    if (VerticalOffsetChanged != null)
                        VerticalOffsetChanged(this, null);
                    break;
            }
        }
    }
}