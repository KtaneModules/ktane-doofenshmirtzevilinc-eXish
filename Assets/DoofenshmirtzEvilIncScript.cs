using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;
using System;

public class DoofenshmirtzEvilIncScript : MonoBehaviour {

    public KMAudio audio;
    public AudioSource jingleaud;
    public AudioClip[] jingleclips;
    public KMSelectable[] buttons;
    public TextMesh numText;
    public SpriteRenderer[] imageRends;
    public Sprite[] images;

    private string[] jingleNames = { "Doofenshmirtz Evil Dirigible!", "Doofenshmirtz Aluminum Siding!", "Doofenshmirtz Evil Annex!", "Doofenshmirtz Abandoned Vacuum Cleaner Factory!", "Doofenshmirtz Abandoned Self Storage!", "Doofenshmirtz Abandoned Theater!", "Doofenshmirtz Evil Incorporated! After Hours...", "Doofenshmirtz House in the suburbs!", "Doofenshmirtz Mentor's Hideout!", "Doofenshmirtz Hideout-Shaped Island!", "Doofenshmirtz out in the forest!", "Doofenshmirtz Quality Bratwurst!", "Doofenshmirtz holding a bucket!", "Doofenshmirtz Evil is Carpeted!", "Doofenshmirtz Family Reunion!", "Poofenplotz's Evil is Crazy!", "Doofenshmirtz Ex-Wife's House in the Hills Somewhere!", "Doofenshmirtz Rocket Powered Jet Skiff!", "Doofenshmirtz flatbed micro-bus!", "Bobblehead Perry the Platypus!", "Doofenshmirtz Carbon Footprint!", "Doofenshmirtz walks to the diner!", "Doofenshmirtz in a jet airplane!", "Doofenshmirtz Wicked Witch Castle!", "Doofenshmirtz, four seconds later!", "Doofenshmirtz's ex-wife's sports sedan!", "Doofenshmirtz in a blimp again!", "Doofenshmirtz Vacation Condo!", "Doofenshmirtz on a crab boat!", "Doofenshmirtz's-- basement.", "Doofenshmirtz Evil News Update!", "Doofenshmirtz's mini bus camping van!", "Bunkalunk Bunkalunk Bunkalunka!", "Doofus Khan's multi-level yurt!", "Doofenshmirtz in his underwear!", "Drusselstein Department of Motor Vehicles and Goat Registration!", "Looking for books at my dad's place!", "Doofenshmirtz Evil Incorporated! Yesterday...", "Doofenshmirtz isn't illuminated!", "Doofenshmirtz Evil Igloo on a Mountaintop, eh?", "Doofenshmirtz...Doofenshmirtz...Doofenshmirtz...Doofenshmirtz...Doofenshmirtz Evil Amalgamated!", "Poofenplotz reading her junk mail!", "Doofenshmirtz at a casino!", "Fully operational Death Star!", "Doofenshmirtz's Motivational Seminar!", "Doofenshmirtz Teenage Girl Movie Night!", "Charlene's No Longer Married to Doofenshmirtz Penthouse!", "Doofenshmirtz hmm-hmm Incorporated!", "Doofenshmirtz Tri-Governor's Mansion!" };
    private string[] jingleInternalNames = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "!", "@", "#", "$", "?", "%", "&", "\\", "(", "{", "[", "<", ";" };
    private List<int> selectedjingles;
    private int currentjingle = 0;
    private int correctjingle = 0;

    private string[][] grid = new string[][]
    {
        new string[] { "-cage", ".", "fan", ".", "-chinese", ".", "scu", ".", "-one", ".", "biggerdrill", ".", "-gown", ".", "slushy" },
        new string[] { ".", "(", ".", "D", ".", "H", ".", "{", ".", "6", ".", "?", ".", "A", "." },
        new string[] { "media", ".", "-rect", ".", "destruct", ".", "-candace", ".", "computer", ".", "-beach", ".", "freeze", ".", "-box" },
        new string[] { ".", "1", ".", "!", ".", "M", ".", "U", ".", "$", ".", "I", ".", "N", "." },
        new string[] { "-locked", ".", "glass", ".", "-silly", ".", "danger", ".", "-theater", ".", "sphere", ".", "-ducttape", ".", "wooden" },
        new string[] { ".", "<", ".", "3", ".", "@", ".", "Z", ".", "%", ".", "9", ".", "0", "." },
        new string[] { "sand", ".", "-choc", ".", "gloom", ".", "-planty", ".", "leaf", ".", "-arm", ".", "ufoish", ".", "-safe" },
        new string[] { ".", "Y", ".", "W", ".", "L", ".", "V", ".", "E", ".", "O", ".", "S", "." },
        new string[] { "-claw", ".", "truck", ".", "-sad", ".", "dragon", ".", "-snowglobe", ".", "suit", ".", "-glass", ".", "queen" },
        new string[] { ".", "R", ".", "F", ".", "4", ".", "#", ".", "P", ".", "T", ".", "8", "." },
        new string[] { "bot", ".", "-net", ".", "drill", ".", "-icetray", ".", "icecream", ".", "-ice", ".", "tv", ".", "-babyseat" },
        new string[] { ".", "C", ".", "G", ".", "[", ".", ";", ".", "Q", ".", "\\", ".", "2", "." },
        new string[] { "-flypaper", ".", "zapper", ".", "-wax", ".", "magnet", ".", "-diaper", ".", "copy", ".", "-crate", ".", "locate" },
        new string[] { ".", "B", ".", "X", ".", "K", ".", "5", ".", "7", ".", "J", ".", "&", "." },
        new string[] { "ugly", ".", "-legwarmer", ".", "norm", ".", "-brainwash", ".", "zing", ".", "-spider", ".", "deflate", ".", "-clawtrap" }
    };
    private int[] selectedimages;

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
        redo:
        selectedimages = new int[2];
        selectedjingles = new List<int>();
        currentjingle = UnityEngine.Random.Range(0, 3);
        if (activated)
            numText.text = "" + (currentjingle + 1);
        int[] x = { -1, -1 }, y = { -1, -1 };
        int[] x2 = { -1, -1, -1 }, y2 = { -1, -1, -1 };
        for (int i = 0; i < 3; i++)
        {
            int rando = UnityEngine.Random.Range(0, jingleclips.Length);
            while (selectedjingles.Contains(rando))
                rando = UnityEngine.Random.Range(0, jingleclips.Length);
            selectedjingles.Add(rando);
            if (i != 2)
            {
                selectedimages[i] = UnityEngine.Random.Range(0, images.Length);
                while (i == 1 && selectedimages[1] == selectedimages[0])
                    selectedimages[i] = UnityEngine.Random.Range(0, images.Length);
                imageRends[i].sprite = images[selectedimages[i]];
            }
            for (int j = 0; j < 15; j++)
            {
                for (int k = 0; k < 15; k++)
                {
                    if (i != 2)
                    {
                        if (grid[j][k] == images[selectedimages[i]].name)
                        {
                            x[i] = j;
                            y[i] = k;
                        }
                    }
                    if (grid[j][k] == jingleInternalNames[selectedjingles[i]])
                    {
                        x2[i] = j;
                        y2[i] = k;
                    }
                }
            }
        }
        int midx;
        int midy;
        if (x[0] > x[1])
            midx = x[0] - ((x[0] - x[1]) / 2);
        else
            midx = x[1] - ((x[1] - x[0]) / 2);
        if (y[0] > y[1])
            midy = y[0] - ((y[0] - y[1]) / 2);
        else
            midy = y[1] - ((y[1] - y[0]) / 2);
        int[] distances = { -1, -1, -1 };
        for (int i = 0; i < 3; i++)
        {
            distances[i] += Math.Abs(midx - x2[i]);
            distances[i] += Math.Abs(midy - y2[i]);
        }
        int min = -1;
        min = Math.Min(distances[0], distances[1]);
        min = Math.Min(min, distances[2]);
        if (distances.Where(e => e == min).Count() > 1)
            goto redo;
        correctjingle = Array.IndexOf(distances, min);
        Debug.LogFormat("[Doofenshmirtz Evil Inc. #{0}] Jingle 1 is \"{1}\".", moduleId, jingleNames[selectedjingles[0]]);
        Debug.LogFormat("[Doofenshmirtz Evil Inc. #{0}] Jingle 2 is \"{1}\".", moduleId, jingleNames[selectedjingles[1]]);
        Debug.LogFormat("[Doofenshmirtz Evil Inc. #{0}] Jingle 3 is \"{1}\".", moduleId, jingleNames[selectedjingles[2]]);
        Debug.LogFormat("[Doofenshmirtz Evil Inc. #{0}] The displayed images are {1} & {2}.", moduleId, images[selectedimages[0]].name, images[selectedimages[1]].name);
        Debug.LogFormat("[Doofenshmirtz Evil Inc. #{0}] After drawing a line between these images, the closest jingle to the midpoint is {1}, which is jingle {2}.", moduleId, jingleInternalNames[selectedjingles[correctjingle]], correctjingle + 1);
    }

    void OnActivate()
    {
        activated = true;
        numText.text = "" + (currentjingle + 1);
    }

    void PressButton(KMSelectable pressed)
    {
        if (moduleSolved != true)
        {
            if (pressed == buttons[0])
            {
                pressed.AddInteractionPunch(0.5f);
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
                pressed.AddInteractionPunch(0.5f);
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
                pressed.AddInteractionPunch(0.5f);
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
                StopCoroutine(countdown);
        }
    }

    private IEnumerator counter()
    {
        while (jingleaud.isPlaying) { yield return null; }
        if (currentjingle == correctjingle)
        {
            Debug.LogFormat("[Doofenshmirtz Evil Inc. #{0}] Submitted jingle {1}, that is correct. Module solved.", moduleId, currentjingle + 1);
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
            while (jingleaud.isPlaying) { yield return "trycancel Cycling of audio halted due to a request to cancel!"; }
            for (int i = 0; i < 2; i++)
            {
                buttons[1].OnInteract();
                yield return new WaitForSeconds(0.1f);
                buttons[2].OnInteract();
                buttons[2].OnInteractEnded();
                while (jingleaud.isPlaying) { yield return "trycancel Cycling of audio halted due to a request to cancel!"; }
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
                    if (currentjingle == correctjingle)
                        yield return "solve";
                    else
                        yield return "strike";
                }
                else
                {
                    yield return "sendtochaterror The specified jingle '" + parameters[1] + "' is not in the range of 1-3!";
                }
            }
            else
            {
                yield return "sendtochaterror!f The specified jingle '" + parameters[1] + "' is invalid!";
            }
            yield break;
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        if (countdown != null && currentjingle != correctjingle)
            buttons[2].OnInteractEnded();
        if (countdown == null)
        {
            if ((currentjingle == 2 && correctjingle == 0) || (currentjingle == 0 && correctjingle == 1) || (currentjingle == 1 && correctjingle == 2))
            {
                buttons[1].OnInteract();
                yield return new WaitForSeconds(0.1f);
            }
            else if ((currentjingle == 2 && correctjingle == 1) || (currentjingle == 1 && correctjingle == 0) || (currentjingle == 0 && correctjingle == 2))
            {
                buttons[0].OnInteract();
                yield return new WaitForSeconds(0.1f);
            }
            buttons[2].OnInteract();
        }
        while (!moduleSolved) { yield return true; }
    }
}
