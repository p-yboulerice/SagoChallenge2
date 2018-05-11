package com.sagosago.sagoapp;

import android.media.AudioManager;
import android.os.Bundle;

import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;

/**
 * Created by minsoo on 2017-07-20.
 */

public class ThalesActivity extends UnityPlayerActivity {

    private AudioManager m_AudioManager;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        m_AudioManager = (AudioManager)this.getSystemService(AUDIO_SERVICE);
        requestAudioFocus();
    }

    @Override
    protected void onStart() {
        super.onStart();
        requestAudioFocus();
    }

    @Override
    protected void onStop() {
        super.onStop();
        abandonAudioFocus();
    }

    @Override
    protected void onResume() {
        super.onResume();
        requestAudioFocus();
    }

    @Override
    protected void onPause() {
        super.onPause();
        abandonAudioFocus();
    }

    private boolean requestAudioFocus() {
        return AudioManager.AUDIOFOCUS_REQUEST_GRANTED == m_AudioManager.requestAudioFocus(m_AudioFocusListener, AudioManager.STREAM_MUSIC, AudioManager.AUDIOFOCUS_GAIN);
    }

    private boolean abandonAudioFocus() {
        return AudioManager.AUDIOFOCUS_REQUEST_GRANTED == m_AudioManager.abandonAudioFocus(m_AudioFocusListener);
    }

    private AudioManager.OnAudioFocusChangeListener m_AudioFocusListener = new AudioManager.OnAudioFocusChangeListener() {
        @Override
        public void onAudioFocusChange(int focusChange) {
            switch(focusChange) {
                case AudioManager.AUDIOFOCUS_GAIN:
                    //UnityMessenger.sendMessage(UnityMessenger.GO_CONTROLLER, UnityMessenger.CONTROLLER_ON_AUDIOFOCUS_CHANGE, UnityMessenger.CONTROLLER_AUDIOFOCUS_GAIN_MESSAGE);
                    break;
                case AudioManager.AUDIOFOCUS_LOSS:
                case AudioManager.AUDIOFOCUS_LOSS_TRANSIENT:
                case AudioManager.AUDIOFOCUS_GAIN_TRANSIENT_MAY_DUCK:
                    //UnityMessenger.sendMessage(UnityMessenger.GO_CONTROLLER, UnityMessenger.CONTROLLER_ON_AUDIOFOCUS_CHANGE, UnityMessenger.CONTROLLER_AUDIOFOCUS_GAIN_MESSAGE);
                    m_AudioManager.abandonAudioFocus(this);
                    break;
            }

        }
    };

}
