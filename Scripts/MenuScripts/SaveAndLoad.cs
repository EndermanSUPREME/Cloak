using System; // convert.To lib
using System.IO; // File Lib
using System.Security.Cryptography; // encyption lib
using System.Text; // strings lib?
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveAndLoad : MonoBehaviour
{
    string saveFileName = @"C:\Cloak_Data\Cloak.txt", saveDir = @"C:\Cloak_Data\";
    [SerializeField] Text playButtonText;
    [SerializeField] int currentLevel, furtherestLevelReached;
    [SerializeField] float setMusicVal, setSFXVal, setResIndex, setResSliderVal;

    [SerializeField] GameObject[] LevelButtons;

    void Awake()
    {
        if (SceneManager.GetActiveScene().buildIndex > 0)
        {
            SaveNewData();
        } else
            {
                CheckForExistingDirectory();
            }
    }

    void CheckForExistingDirectory()
    {
        if (!Directory.Exists(saveDir)) // no save dir on system
        {
            Directory.CreateDirectory(saveDir);
        }

        Invoke("CheckForExistingFile", 0.05f);
    }

    /* File Content Style =>  furtherestLevelReached, music, sfx, resIndex, resSlider */

    void CheckForExistingFile()
    {
        if (File.Exists(saveFileName)) // grab the data if we have a file
        {
            if (playButtonText != null)
            {
                playButtonText.text = "Continue";
                playButtonText.fontSize = playButtonText.fontSize - 10;
            }

            // print("[+] Save File Detected. . .");
            TextReader tr = new StreamReader(saveFileName); // the TextReader reads the txt file and reads the lines to then be converted to game data
            // tr.ReadLine();

            furtherestLevelReached = Convert.ToInt32(Base64Decode(tr.ReadLine()));
            setMusicVal = (float)Convert.ToDouble(Base64Decode(tr.ReadLine()));
            setSFXVal = (float)Convert.ToDouble(Base64Decode(tr.ReadLine()));
            setResIndex = (float)Convert.ToDouble(Base64Decode(tr.ReadLine()));
            setResSliderVal = (float)Convert.ToDouble(Base64Decode(tr.ReadLine()));

            PlayerPrefs.SetInt("furthestLevelReached", furtherestLevelReached);
            PlayerPrefs.SetFloat("MusicVol", setMusicVal);
            PlayerPrefs.SetFloat("SFXVol", setSFXVal);
            PlayerPrefs.SetFloat("ResSlideVal", setResSliderVal);

            tr.Close();

            GetComponent<MenuScript>().ApplyPlayerSettings();
        } else // make a file and set the defaults
            {
                if (playButtonText != null)
                {
                    playButtonText.text = "Play";
                }

                TextWriter tw = new StreamWriter(saveFileName); // the TextReader writes data into the save file
                // tw.WriteLine();

                tw.WriteLine(Base64Encode("1"));
                tw.WriteLine(Base64Encode("0.5"));
                tw.WriteLine(Base64Encode("0.5"));
                tw.WriteLine(Base64Encode("5"));
                tw.WriteLine(Base64Encode("5"));

                PlayerPrefs.SetInt("furthestLevelReached", 1);
                PlayerPrefs.SetFloat("MusicVol", 0.5f);
                PlayerPrefs.SetFloat("SFXVol", 0.5f);
                PlayerPrefs.SetFloat("ResSlideVal", 5);

                furtherestLevelReached = PlayerPrefs.GetInt("furthestLevelReached");
                setMusicVal = PlayerPrefs.GetFloat("MusicVol"); // [0-1]
                setSFXVal = PlayerPrefs.GetFloat("SFXVol"); // [0-1]
                setResIndex = PlayerPrefs.GetFloat("ResSlideVal");
                setResSliderVal = PlayerPrefs.GetFloat("ResSlideVal");

                tw.Close();
            }
    }

    string Base64Encode(string info)
    {
        var infoBytes = System.Text.Encoding.UTF8.GetBytes(info);
        return System.Convert.ToBase64String(infoBytes);
    }

    string Base64Decode(string base64EncodedData)
    {
        var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }

    public void SaveNewData() // runs during each level
    {
        Debug.Log("[+] Saving New Data");

        currentLevel = SceneManager.GetActiveScene().buildIndex;

        if (currentLevel > furtherestLevelReached)
        {
            furtherestLevelReached = currentLevel;
        }

        PlayerPrefs.SetInt("furthestLevelReached", furtherestLevelReached);
        setMusicVal = PlayerPrefs.GetFloat("MusicVol"); // [0-1]
        setSFXVal = PlayerPrefs.GetFloat("SFXVol"); // [0-1]
        setResIndex = PlayerPrefs.GetFloat("ResSlideVal");
        setResSliderVal = PlayerPrefs.GetFloat("ResSlideVal");

        TextWriter tw = new StreamWriter(saveFileName); // the TextReader writes data into the save file
        // tw.WriteLine();

        tw.WriteLine(Base64Encode(furtherestLevelReached.ToString()));
        tw.WriteLine(Base64Encode(setMusicVal.ToString()));
        tw.WriteLine(Base64Encode(setSFXVal.ToString()));
        tw.WriteLine(Base64Encode(setResIndex.ToString()));
        tw.WriteLine(Base64Encode(setResSliderVal.ToString()));

        tw.Close();
    }

//======================================================================================================================================================

    public void GetAvaliableLevels()
    {
        for (int i = 0; i < LevelButtons.Length; i++)
        {
            if (i < PlayerPrefs.GetInt("furthestLevelReached"))
            {
                LevelButtons[i].SetActive(true);
            } else
                {
                    LevelButtons[i].SetActive(false);
                }
        }
    }

}//EndScript