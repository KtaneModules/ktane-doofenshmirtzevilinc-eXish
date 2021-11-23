using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using System.Text.RegularExpressions;
using System;

public class DoofenshmirtzEvilIncScript : MonoBehaviour {

    public KMAudio audio;
    public KMBombInfo bomb;
    public AudioSource jingleaud;
    public AudioClip[] jingleclips;
    public KMSelectable[] buttons;
    public TextMesh numText;

    private string[] jingleNames = { "Doofenshmirtz Evil Incorporated!", "Doofenshmirtz holding a bucket!", "Doofenshmirtz Quality Bratwurst!", "Doofenshmirtz flatbed micro-bus!", "Doofenshmirtz's ex-wife's sports sedan!", "Doofenshmirtz Abandoned Theater!", "Doofenshmirtz Carbon Footprint!", "Fully operational Death Star!", "Bunkalunk Bunkalunk Bunkalunka!" };
    private int[] jingleValues = { 6, 2, 7, 10, 2, 5, 9, 1, 10 };
    private int[] selectedjingles;
    private int currentjingle = 0;
    private int correctjingle = 0;

    private string[] trapNames = { "Evil Bubble", "Egg Trap", "Welcome Mat", "Bottle", "Spray Chocolate", "Giant Metal Apple" };
    private int selectedtrap = 0;

    private string[] inatorNames = { "Ugly-inator", "Drill-inator", "Stain-inator", "Scorch-inator", "Junk Food-inator", "Eye-Fog-inator" };
    private int selectedinator = 0;

    private bool activated = false;

    private Coroutine countdown;

    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        moduleSolved = false;
        foreach (KMSelectable obj in buttons)
        {
            KMSelectable pressed = obj;
            pressed.OnInteract += delegate () { PressButton(pressed); return false; };
            pressed.OnInteractEnded += delegate () { ReleaseButton(pressed); };
        }
        numText.text = "";
        GetComponent<KMBombModule>().OnActivate += OnActivate;
    }

    void Start () {
        selectJingles();
        selectTrapAndInator();
        calculateCorrectJingle();
    }

    void OnActivate()
    {
        activated = true;
        numText.text = "" + (currentjingle + 1);
    }

    void PressButton(KMSelectable pressed)
    {
        if(moduleSolved != true)
        {
            if (pressed == buttons[0])
            {
                pressed.AddInteractionPunch(0.25f);
                audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, pressed.transform);
                currentjingle--;
                if (currentjingle == -1)
                {
                    currentjingle = 2;
                }
                numText.text = "" + (currentjingle + 1);
            }
            else if (pressed == buttons[1])
            {
                pressed.AddInteractionPunch(0.25f);
                audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, pressed.transform);
                currentjingle++;
                if (currentjingle == 3)
                {
                    currentjingle = 0;
                }
                numText.text = "" + (currentjingle + 1);
            }
            else if (pressed == buttons[2])
            {
                pressed.AddInteractionPunch(0.25f);
                jingleaud.clip = jingleclips[selectedjingles[currentjingle]];
                jingleaud.Play();
                countdown = StartCoroutine(counter());
            }
        }
    }

    private void ReleaseButton(KMSelectable released)
    {
        if (moduleSolved != true)
        {
            if (released == buttons[2])
            {
                StopCoroutine(countdown);
            }
        }
    }

    private void selectJingles()
    {
        selectedjingles = new int[3];
        while (selectedjingles[0] == selectedjingles[1] || selectedjingles[1] == selectedjingles[2] || selectedjingles[2] == selectedjingles[0])
        {
            selectedjingles[0] = UnityEngine.Random.Range(0, jingleNames.Length);
            selectedjingles[1] = UnityEngine.Random.Range(0, jingleNames.Length);
            selectedjingles[2] = UnityEngine.Random.Range(0, jingleNames.Length);
        }
        Debug.LogFormat("[Doofenshmirtz Evil Inc. #{0}] Jingle 1 is \"{1}\", with the associated number {2}", moduleId, jingleNames[selectedjingles[0]], jingleValues[selectedjingles[0]]);
        Debug.LogFormat("[Doofenshmirtz Evil Inc. #{0}] Jingle 2 is \"{1}\", with the associated number {2}", moduleId, jingleNames[selectedjingles[1]], jingleValues[selectedjingles[1]]);
        Debug.LogFormat("[Doofenshmirtz Evil Inc. #{0}] Jingle 3 is \"{1}\", with the associated number {2}", moduleId, jingleNames[selectedjingles[2]], jingleValues[selectedjingles[2]]);
        currentjingle = UnityEngine.Random.Range(0, 3);
        if (activated)
            numText.text = "" + (currentjingle + 1);
    }

    private void selectTrapAndInator()
    {
        selectedtrap = UnityEngine.Random.Range(0, trapNames.Length);
        Debug.LogFormat("[Doofenshmirtz Evil Inc. #{0}] The displayed trap is {1}", moduleId, trapNames[selectedtrap]);
        selectedinator = UnityEngine.Random.Range(0, inatorNames.Length);
        Debug.LogFormat("[Doofenshmirtz Evil Inc. #{0}] The displayed \"-inator\" is the {1}", moduleId, inatorNames[selectedinator]);
    }

    private void calculateCorrectJingle()
    {
        int sum = 0;
        string selecttrap = trapNames[selectedtrap].Replace(" ", "");
        if (selecttrap.Length < bomb.GetBatteryCount())
        {
            Debug.LogFormat("[Doofenshmirtz Evil Inc. #{0}] The number of letters in the trap's name is less than the number of batteries! Summing all letters...", moduleId);
            string summed = "";
            for (int i = 0; i < selecttrap.Length; i++)
            {
                sum += char.ToUpper(selecttrap.ElementAt(i)) - 64;
                summed += char.ToUpper(selecttrap.ElementAt(i));
            }
            Debug.LogFormat("[Doofenshmirtz Evil Inc. #{0}] The summed letters were: {1}", moduleId, summed);
        }
        else if (bomb.GetSerialNumberNumbers().Last() % 2 == 0)
        {
            Debug.LogFormat("[Doofenshmirtz Evil Inc. #{0}] The bomb's serial number has a last digit of {1} (even), summing the first {2} letters!", moduleId, bomb.GetSerialNumberNumbers().Last(), bomb.GetBatteryCount());
            string summed = "";
            for (int i = 0; i < bomb.GetBatteryCount(); i++)
            {
                sum += char.ToUpper(selecttrap.ElementAt(i)) - 64;
                summed += char.ToUpper(selecttrap.ElementAt(i));
            }
            if (summed.Length == 0)
                Debug.LogFormat("[Doofenshmirtz Evil Inc. #{0}] The summed letters were: none", moduleId);
            else
                Debug.LogFormat("[Doofenshmirtz Evil Inc. #{0}] The summed letters were: {1}", moduleId, summed);
        }
        else
        {
            Debug.LogFormat("[Doofenshmirtz Evil Inc. #{0}] The bomb's serial number has a last digit of {1} (odd), summing the last {2} letters!", moduleId, bomb.GetSerialNumberNumbers().Last(), bomb.GetBatteryCount());
            string summed = "";
            for (int i = selecttrap.Length-1; i >= selecttrap.Length-bomb.GetBatteryCount(); i--)
            {
                sum += char.ToUpper(selecttrap.ElementAt(i)) - 64;
                summed = summed.Insert(0, "" + char.ToUpper(selecttrap.ElementAt(i)));
            }
            if (summed.Length == 0)
                Debug.LogFormat("[Doofenshmirtz Evil Inc. #{0}] The summed letters were: none", moduleId);
            else
                Debug.LogFormat("[Doofenshmirtz Evil Inc. #{0}] The summed letters were: {1}", moduleId, summed);
        }
        Debug.LogFormat("[Doofenshmirtz Evil Inc. #{0}] The sum after trap calculations is now {1}", moduleId, sum);
        char[] vowels = { 'A', 'E', 'I', 'O', 'U' };
        if (bomb.IsIndicatorPresent("FRK") || bomb.IsIndicatorPresent("SND"))
        {
            int count = 0;
            for (int i = 0; i < inatorNames[selectedinator].Replace(" ", "").Replace("-", "").Length; i++)
            {
                if (vowels.Contains(char.ToUpper(inatorNames[selectedinator].Replace(" ", "").Replace("-", "").ElementAt(i))))
                {
                    sum += 1;
                    count++;
                }
            }
            Debug.LogFormat("[Doofenshmirtz Evil Inc. #{0}] There is an FRK or SND indicator present! The number of vowels present in the \"-inator\"'s name is {1}", moduleId, count);
        }
        else
        {
            int count = 0;
            for (int i = 0; i < inatorNames[selectedinator].Replace(" ", "").Replace("-", "").Length; i++)
            {
                if (!vowels.Contains(char.ToUpper(inatorNames[selectedinator].Replace(" ", "").Replace("-", "").ElementAt(i))))
                {
                    sum += 1;
                    count++;
                }
            }
            Debug.LogFormat("[Doofenshmirtz Evil Inc. #{0}] There is not an FRK or SND indicator present! The number of consonants present in the \"-inator\"'s name is {1}", moduleId, count);
        }
        Debug.LogFormat("[Doofenshmirtz Evil Inc. #{0}] The sum after \"-inator\" calculations is now {1}", moduleId, sum);
        if (bomb.GetSerialNumberNumbers().Contains(1))
        {
            sum *= jingleValues[selectedjingles[0]];
            Debug.LogFormat("[Doofenshmirtz Evil Inc. #{0}] The bomb's serial number contains a 1! Multiplying the sum by the number associated with the first jingle!", moduleId);
        }
        else if (bomb.GetSerialNumberNumbers().Contains(2))
        {
            sum *= jingleValues[selectedjingles[1]];
            Debug.LogFormat("[Doofenshmirtz Evil Inc. #{0}] The bomb's serial number contains a 2! Multiplying the sum by the number associated with the second jingle!", moduleId);
        }
        else if (bomb.GetSerialNumberNumbers().Contains(3))
        {
            sum *= jingleValues[selectedjingles[2]];
            Debug.LogFormat("[Doofenshmirtz Evil Inc. #{0}] The bomb's serial number contains a 3! Multiplying the sum by the number associated with the third jingle!", moduleId);
        }
        //Replace 99 with index of "Doofenshmirtz walks to diner!"
        else if (selectedjingles[0] == 99 || selectedjingles[1] == 99 || selectedjingles[2] == 99)
        {
            sum *= jingleValues[selectedjingles[0]];
            Debug.LogFormat("[Doofenshmirtz Evil Inc. #{0}] The jingle \"Doofenshmirtz walks to the diner!\" is an option! Multiplying the sum by the number associated with the first jingle!", moduleId);
        }
        else if (selectedtrap == 4 || selectedtrap == 5)
        {
            sum *= jingleValues[selectedjingles[1]];
            Debug.LogFormat("[Doofenshmirtz Evil Inc. #{0}] The trap Spray Chocolate or Giant Metal Apple is displayed! Multiplying the sum by the number associated with the second jingle!", moduleId);
        }
        else
        {
            sum *= jingleValues[selectedjingles[2]];
            Debug.LogFormat("[Doofenshmirtz Evil Inc. #{0}] None of the conditions applied! Multiplying the sum by the number associated with the third jingle!", moduleId);
        }
        Debug.LogFormat("[Doofenshmirtz Evil Inc. #{0}] The sum after multiplication calculations is now {1}", moduleId, sum);
        int temp = sum;
        sum %= 3;
        correctjingle = sum;
        Debug.LogFormat("[Doofenshmirtz Evil Inc. #{0}] The correct jingle is {1} % 3 = {2} + 1 = {3}", moduleId, temp, temp %= 3, temp += 1);
    }

    private IEnumerator counter()
    {
        while (jingleaud.isPlaying) { yield return new WaitForSeconds(0.1f); }
        if (currentjingle == correctjingle)
        {
            Debug.LogFormat("[Doofenshmirtz Evil Inc. #{0}] Submitted jingle {1}, that is correct. Module disarmed.", moduleId, currentjingle+1);
            moduleSolved = true;
            numText.text = "";
            GetComponent<KMBombModule>().HandlePass();
        }
        else
        {
            Debug.LogFormat("[Doofenshmirtz Evil Inc. #{0}] Submitted jingle {1}, that is incorrect. Strike! Module resetting...", moduleId, currentjingle + 1);
            GetComponent<KMBombModule>().HandleStrike();
            Start();
        }
    }

    //twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} cycle [Cycles through all three jingles] | !{0} submit <#> [Submits the specified jingle (1-3)]";
    #pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        if (Regex.IsMatch(command, @"^\s*cycle\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (currentjingle == 2)
            {
                buttons[1].OnInteract();
                yield return new WaitForSeconds(0.1f);
            }
            else if (currentjingle == 1)
            {
                buttons[0].OnInteract();
                yield return new WaitForSeconds(0.1f);
            }
            buttons[2].OnInteract();
            buttons[2].OnInteractEnded();
            while (jingleaud.isPlaying) { yield return "trycancel Cycling of audio halted due to a request to cancel!"; yield return new WaitForSeconds(0.1f); }
            for (int i = 0; i < 2; i++)
            {
                buttons[1].OnInteract();
                yield return new WaitForSeconds(0.1f);
                buttons[2].OnInteract();
                buttons[2].OnInteractEnded();
                while (jingleaud.isPlaying) { yield return "trycancel Cycling of audio halted due to a request to cancel!"; yield return new WaitForSeconds(0.1f); }
            }
            buttons[1].OnInteract();
            yield break;
        }
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*submit\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (parameters.Length > 2)
            {
                yield return "sendtochaterror Too many parameters!";
                yield break;
            }
            else if (parameters.Length == 1)
            {
                yield return "sendtochaterror Please specify the jingle you would like to submit!";
                yield break;
            }
            int check = 0;
            if (int.TryParse(parameters[1], out check))
            {
                if (check > 0 && check < 4)
                {
                    if ((currentjingle == 2 && check == 1) || (currentjingle == 0 && check == 2) || (currentjingle == 1 && check == 3))
                    {
                        buttons[1].OnInteract();
                        yield return new WaitForSeconds(0.1f);
                    }
                    else if ((currentjingle == 2 && check == 2) || (currentjingle == 1 && check == 1) || (currentjingle == 0 && check == 3))
                    {
                        buttons[0].OnInteract();
                        yield return new WaitForSeconds(0.1f);
                    }
                    buttons[2].OnInteract();
                }
                else
                {
                    yield return "sendtochaterror The specified jingle '" + parameters[1] + "' is not in the range of 1-3!";
                }
            }
            else
            {
                yield return "sendtochaterror The specified jingle '" + parameters[1] + "' is invalid!";
            }
            yield break;
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        if (countdown == null)
        {
            yield return ProcessTwitchCommand("submit "+(correctjingle+1));
        }
        buttons[2].OnInteract();
        while (!moduleSolved) { yield return true; yield return new WaitForSeconds(0.1f); }
    }
}
