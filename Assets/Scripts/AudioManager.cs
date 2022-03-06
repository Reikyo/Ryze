using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public class SfxClpVol
    {
        private AudioSource audioSource;

        public AudioClip sfxclp;
        public float fVolume;

        public SfxClpVol(AudioClip sfxclpGiven, float fVolumeGiven=1f)
        {
            audioSource = GameObject.Find("Audio Manager").GetComponent<AudioSource>();
            sfxclp = sfxclpGiven;
            fVolume = fVolumeGiven;
        }

        public void PlayOneShot()
        {
            audioSource.PlayOneShot(sfxclp, fVolume);
        }
    }

    // ------------------------------------------------------------------------------------------------

    private AudioSource audioSource;

    private string sPathAudio = "Assets/Asset Store/Audio/";

    public int iIdx_sfxclpvolListMusic = 0;
    public List<SfxClpVol> sfxclpvolListMusic = new List<SfxClpVol>();

    public SfxClpVol sfxclpvolUIScroll;
    public SfxClpVol sfxclpvolUISubmit;
    public SfxClpVol sfxclpvolUICancel;

    public List<SfxClpVol> sfxclpvolListProjectilePlayer = new List<SfxClpVol>();
    public List<SfxClpVol> sfxclpvolListProjectileEnemy = new List<SfxClpVol>();

    public SfxClpVol sfxclpvolPowerUpHealth;
    public SfxClpVol sfxclpvolPowerUpCharge;

    public SfxClpVol sfxclpvolExplosionAsteroid;
    public SfxClpVol sfxclpvolExplosionEnemy;

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();

        sfxclpvolListMusic.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "Neocrey/Free Music Bundle/System Shock.mp3", typeof(AudioClip)), 0.1f));
        sfxclpvolListMusic.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "Neocrey/Free Music Bundle/Burn In Space.mp3", typeof(AudioClip)), 0.1f));
        sfxclpvolListMusic.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "Neocrey/Free Music Bundle/Cyberspace Hunters.mp3", typeof(AudioClip)), 0.1f));
        sfxclpvolListMusic.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "Neocrey/Free Music Bundle/Destractor.mp3", typeof(AudioClip)), 0.1f));
        sfxclpvolListMusic.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "Neocrey/Free Music Bundle/Disco Century.mp3", typeof(AudioClip)), 0.1f));
        sfxclpvolListMusic.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "Neocrey/Free Music Bundle/Fractal.mp3", typeof(AudioClip)), 0.1f));
        sfxclpvolListMusic.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "Neocrey/Free Music Bundle/Frozy.mp3", typeof(AudioClip)), 0.1f));
        sfxclpvolListMusic.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "Neocrey/Free Music Bundle/Heart open.mp3", typeof(AudioClip)), 0.1f));
        sfxclpvolListMusic.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "Neocrey/Free Music Bundle/Jump to win.mp3", typeof(AudioClip)), 0.1f));
        sfxclpvolListMusic.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "Neocrey/Free Music Bundle/Laser Millenium.mp3", typeof(AudioClip)), 0.1f));
        sfxclpvolListMusic.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "Neocrey/Free Music Bundle/Miami Soul.mp3", typeof(AudioClip)), 0.1f));
        sfxclpvolListMusic.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "Neocrey/Free Music Bundle/NEON.mp3", typeof(AudioClip)), 0.1f));
        sfxclpvolListMusic.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "Neocrey/Free Music Bundle/Nightwind.mp3", typeof(AudioClip)), 0.1f));
        sfxclpvolListMusic.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "Neocrey/Free Music Bundle/Rise.mp3", typeof(AudioClip)), 0.1f));
        sfxclpvolListMusic.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "Neocrey/Free Music Bundle/Stars.mp3", typeof(AudioClip)), 0.1f));
        sfxclpvolListMusic.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "Neocrey/Free Music Bundle/Twilight.mp3", typeof(AudioClip)), 0.1f));
        sfxclpvolListMusic.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "Neocrey/Free Music Bundle/Warrior Song.mp3", typeof(AudioClip)), 0.1f));
        sfxclpvolListMusic.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "Neocrey/Free Music Bundle/Your personal heaven.mp3", typeof(AudioClip)), 0.1f));

        sfxclpvolUIScroll = new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "Little Robot Sound Factory/UI Sfx/Mp3/Click_Electronic/Click_Electronic_14.mp3", typeof(AudioClip)), 0.5f);
        sfxclpvolUISubmit = new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "Little Robot Sound Factory/UI Sfx/Mp3/Click_Electronic/Click_Electronic_12.mp3", typeof(AudioClip)), 0.5f);
        sfxclpvolUICancel = new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "Little Robot Sound Factory/UI Sfx/Mp3/Click_Electronic/Click_Electronic_13.mp3", typeof(AudioClip)), 0.5f);

        sfxclpvolListProjectilePlayer.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "Shapeforms/Shapeforms Audio Free Sound Effects/Type Preview/AUDIO/Tablet_Swipe_01.wav", typeof(AudioClip)), 0.75f));
        // sfxclpvolListProjectilePlayer.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "Shapeforms/Shapeforms Audio Free Sound Effects/Hit and Punch Preview/AUDIO/WHOOSH_AIRY_FLUTTER_01.wav", typeof(AudioClip)), 0.75f)); // A bit too harsh for constant firing
        // sfxclpvolListProjectilePlayer.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "MGWSoundDesign/FuturisticGunSoundFX/Laser/Laser10.wav", typeof(AudioClip)), 0.75f)); // A bit too harsh for constant firing
        // sfxclpvolListProjectilePlayer.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "MGWSoundDesign/FuturisticGunSoundFX/Laser/Laser20.wav", typeof(AudioClip)), 0.75f)); // A bit too harsh for constant firing
        // sfxclpvolListProjectilePlayer.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "Little Robot Sound Factory/UI Sfx/Mp3/Click_Electronic/Click_Electronic_05.mp3", typeof(AudioClip)), 0.75f)); // Okay, but a bit too chirpy like a little bird
        // sfxclpvolListProjectilePlayer.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "Little Robot Sound Factory/UI Sfx/Mp3/Click_Standard_00.mp3", typeof(AudioClip)), 0.75f)); // Okay, but a bit too clicky like a typewriter

        sfxclpvolListProjectileEnemy.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "MGW Sound Design/FuturisticGunSoundFX/Laser/Laser6.wav", typeof(AudioClip)), 0.1f));
        sfxclpvolListProjectileEnemy.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "MGW Sound Design/FuturisticGunSoundFX/Laser/Laser7.wav", typeof(AudioClip)), 0.1f));
        sfxclpvolListProjectileEnemy.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "MGW Sound Design/FuturisticGunSoundFX/Laser/Laser8.wav", typeof(AudioClip)), 0.1f));
        sfxclpvolListProjectileEnemy.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "MGW Sound Design/FuturisticGunSoundFX/Laser/Laser9.wav", typeof(AudioClip)), 0.1f));
        sfxclpvolListProjectileEnemy.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "MGW Sound Design/FuturisticGunSoundFX/Laser/Laser10.wav", typeof(AudioClip)), 0.1f));
        sfxclpvolListProjectileEnemy.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "MGW Sound Design/FuturisticGunSoundFX/Laser/Laser16.wav", typeof(AudioClip)), 0.1f));
        sfxclpvolListProjectileEnemy.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "MGW Sound Design/FuturisticGunSoundFX/Laser/Laser17.wav", typeof(AudioClip)), 0.1f));
        sfxclpvolListProjectileEnemy.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "MGW Sound Design/FuturisticGunSoundFX/Laser/Laser18.wav", typeof(AudioClip)), 0.1f));
        sfxclpvolListProjectileEnemy.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "MGW Sound Design/FuturisticGunSoundFX/Laser/Laser19.wav", typeof(AudioClip)), 0.1f));
        sfxclpvolListProjectileEnemy.Add(new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "MGW Sound Design/FuturisticGunSoundFX/Laser/Laser20.wav", typeof(AudioClip)), 0.1f));

        sfxclpvolPowerUpHealth = new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "Little Robot Sound Factory/Electric Sfx/Mp3/Jingle_Win_Synth/Jingle_Win_Synth_03.mp3", typeof(AudioClip)), 0.5f);
        sfxclpvolPowerUpCharge = new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "Little Robot Sound Factory/Electric Sfx/Mp3/Jingle_Win_Synth/Jingle_Win_Synth_04.mp3", typeof(AudioClip)), 0.5f);

        sfxclpvolExplosionAsteroid = new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "Shapeforms/Shapeforms Audio Free Sound Effects/Sci Fi Weapons Cyberpunk Arsenal Preview/AUDIO/EXPLDsgn_Explosion Rumble Distorted_01.wav", typeof(AudioClip)), 0.1f);
        sfxclpvolExplosionEnemy = new SfxClpVol((AudioClip)AssetDatabase.LoadAssetAtPath(sPathAudio + "Shapeforms/Shapeforms Audio Free Sound Effects/Sci Fi Weapons Cyberpunk Arsenal Preview/AUDIO/EXPLDsgn_Implode_15.wav", typeof(AudioClip)), 0.1f);
    }

    // ------------------------------------------------------------------------------------------------

    void Update()
    {
        // Enable the following for music:
        // if (!audioSource.isPlaying)
        // {
        //     sfxclpvolListMusic[iIdx_sfxclpvolListMusic].PlayOneShot();
        //     if (iIdx_sfxclpvolListMusic < (sfxclpvolListMusic.Count - 1))
        //     {
        //         iIdx_sfxclpvolListMusic += 1;
        //     }
        //     else
        //     {
        //         iIdx_sfxclpvolListMusic = 0;
        //     }
        // }
    }

    // ------------------------------------------------------------------------------------------------

    // public void PlayOneShot(SfxClpVol sfxclpvol)
    // {
    //     audioSource.PlayOneShot(sfxclpvol.sfxclp, sfxclpvol.fVolume);
    // }

    // ------------------------------------------------------------------------------------------------

}
