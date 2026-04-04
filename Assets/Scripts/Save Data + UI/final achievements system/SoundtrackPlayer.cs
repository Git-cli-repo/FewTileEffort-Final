using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using TMPro;

public class SoundtrackPlayer : MonoBehaviour
{
    public enum SongNames
    {
        TimeToTutorial,
        AdventureStart,
        TheSwitchLand,
        BulletHell,
        MovingAhead,
        UphillClimb,
        ANewWorld,
        Smol,
        Familiarity,
        RedBluePalace,
        TheEnd,
        FewTileEffort,
        Squash,
        Checklist,
        BeginAgain,
        JustPressZ,
        Backtracking,
        TheLightNeverFades,
        PerceptionShift,
        Intermission,
        ToSwingInSepia,
        ShiningFortress,
        TheFinalMountain,
        YouveComeSoFar,
        Confusion,
        BrokenBones,
        PushForward,
        EndOfTheLine,
        Investigation,
        DestinyAwaits,
        HauntedEcho,
        ChamberBells,
        DifficultySpiral,
        ChasingTheEnd,
        ProlongedSuffering,
        TheFinalChallenge,
        TheTrueCompletionist
    };
    public AudioClip timeToTutorial;
    public AudioClip adventureStart;
    public AudioClip theSwitchLand;
    public AudioClip bulletHell;
    public AudioClip movingAhead;
    public AudioClip uphillClimb;
    public AudioClip aNewWorld;
    public AudioClip smol;
    public AudioClip familiarity;
    public AudioClip redBluePalace;
    public AudioClip theEnd;
    public AudioClip fewTileEffort;
    public AudioClip squash;
    public AudioClip checklist;
    public AudioClip beginAgain;
    public AudioClip justPressZ;
    public AudioClip backtracking;
    public AudioClip theLightNeverFades;
    public AudioClip perceptionShift;
    public AudioClip intermission;
    public AudioClip toSwingInSepia;
    public AudioClip shiningFortress;
    public AudioClip theFinalMountain;
    public AudioClip youveComeSoFar;
    public AudioClip confusion;
    public AudioClip brokenBones;
    public AudioClip pushForward;
    public AudioClip endOfTheLine;
    public AudioClip investigation;
    public AudioClip destinyAwaits;
    public AudioClip hauntedEcho;
    public AudioClip chamberBells;
    public AudioClip difficultySpiral;
    public AudioClip chasingTheEnd;
    public AudioClip prolongedSuffering;
    public AudioClip theFinalChallenge;
    public AudioClip theTrueCompletionist;
    public AudioSource source;
    public Dictionary<SongNames, AudioClip> songRef;
    public SongNames playThisSong;
    public TMP_Text nowPlaying;
    public float time;
    public float totalTime;

    public void Start()
    {
        songRef = new Dictionary<SongNames, AudioClip>
        {
            { SongNames.TimeToTutorial, timeToTutorial },
            { SongNames.AdventureStart, adventureStart },
            { SongNames.TheSwitchLand, theSwitchLand },
            { SongNames.BulletHell, bulletHell },
            { SongNames.MovingAhead, movingAhead },
            { SongNames.UphillClimb, uphillClimb },
            { SongNames.ANewWorld, aNewWorld },
            { SongNames.Smol, smol },
            { SongNames.Familiarity, familiarity },
            { SongNames.RedBluePalace, redBluePalace },
            { SongNames.TheEnd, theEnd },
            { SongNames.FewTileEffort, fewTileEffort },
            { SongNames.Squash, squash },
            { SongNames.Checklist, checklist },
            { SongNames.BeginAgain, beginAgain },
            { SongNames.JustPressZ, justPressZ },
            { SongNames.Backtracking, backtracking },
            { SongNames.TheLightNeverFades, theLightNeverFades },
            { SongNames.PerceptionShift, perceptionShift },
            { SongNames.Intermission, intermission },
            { SongNames.ToSwingInSepia, toSwingInSepia },
            { SongNames.ShiningFortress, shiningFortress },
            { SongNames.TheFinalMountain, theFinalMountain },
            { SongNames.YouveComeSoFar, youveComeSoFar },
            { SongNames.Confusion, confusion },
            { SongNames.BrokenBones, brokenBones },
            { SongNames.PushForward, pushForward },
            { SongNames.EndOfTheLine, endOfTheLine },
            { SongNames.Investigation, investigation },
            { SongNames.DestinyAwaits, destinyAwaits },
            { SongNames.HauntedEcho, hauntedEcho },
            { SongNames.ChamberBells, chamberBells },
            { SongNames.DifficultySpiral, difficultySpiral },
            { SongNames.ChasingTheEnd, chasingTheEnd },
            { SongNames.ProlongedSuffering, prolongedSuffering },
            { SongNames.TheFinalChallenge, theFinalChallenge },
            { SongNames.TheTrueCompletionist, theTrueCompletionist }
        };

        source = FindObjectsByType<GameObject>(FindObjectsSortMode.None).FirstOrDefault(p => p.name == "RunManager")?.GetComponent<AudioSource>();
        source.Stop();
        nowPlaying.text = "";

    }

    System.Collections.IEnumerator Crossfade(AudioSource src, AudioClip next, float dur = 0.4f)
    {
        float startVol = src.volume;
        for (float t = 0; t < dur; t += Time.unscaledDeltaTime)
        {
            src.volume = Mathf.Lerp(startVol, 0f, t / dur);
            yield return null;
        }
        src.Stop();
        src.clip = next;
        src.Play();
        for (float t = 0; t < dur; t += Time.unscaledDeltaTime)
        {
            src.volume = Mathf.Lerp(0f, startVol, t / dur);
            yield return null;
        }
        src.volume = startVol;
    }


    public void PlaySong()
    {
        StartCoroutine(Crossfade(source, songRef[playThisSong]));
        time = 0f;
    }

    public void Update()
    {
        if (source.isPlaying)
        {
            time += Time.deltaTime;
            totalTime += Time.deltaTime;
        }
        nowPlaying.text = $"NOW PLAYING: {source.clip.name} for {Mathf.FloorToInt(time / 60):00}:{Mathf.FloorToInt(time % 60)}! YOU'VE LISTENED FOR {Mathf.FloorToInt(totalTime / 60):00}:{Mathf.FloorToInt(totalTime % 60)}!!";
    }
}
