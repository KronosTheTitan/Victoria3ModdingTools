//#define EMPIRE
//#define HEGEMONY
#define KINGDOM
#define GRANDPRINCIPALITY
#define PRINCIPALITY
//#define CITYSTATE

#define DEBUGMODE

using System.IO;

struct Country
{
    public string Name;
    public string Adj;
    public string Tag;
    public Tier Tier;

    #if DEBUGMODE
    public void PrintCountry()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("---NEW COUNTRY---");
        Console.WriteLine(Name);
        Console.WriteLine(Adj);
        Console.WriteLine(Tag);
        Console.WriteLine(Tier);
        Console.ResetColor();
    }
    #endif
}

enum Tier
{
    Empire,
    Hegemony,
    Kingdom,
    GrandPrincipality,
    Principality,
    CityState
}

class Program
{
    private static List<string> _namesFile;
    private static List<string> _logicFile;
    private static Dictionary<string, Country> _countries = new Dictionary<string, Country>();
    private static List<string> _tags = new List<string>();

    static void Main(string[] args)
    {
        Console.WriteLine("---Start---");

        string[] initNamesFile = System.IO.File.ReadAllLines("./Input/countries_l_english.yml");
        string[] initLogicFile = System.IO.File.ReadAllLines("./input/00_dynamic_country_names.txt");
        _namesFile = initNamesFile.ToList();
        _logicFile = initLogicFile.ToList();

        string[] tiers = File.ReadAllText("./Input/TierList.txt").Split(":");
        
        foreach (string line in _namesFile)
        {
            if (line == "  # Dynamic countries") break;
            if(line == "l_english:") continue;

            string[] splitters = { ":0 ", ":1 " };

            string[] segments = line.Split(splitters,StringSplitOptions.TrimEntries);
            string[] segments2 = segments[0].Split("_");
            
            if (segments2.Length == 2)
            {
                string adjective = segments[1].Replace("\"","");
                Country country = _countries[segments2[0]];
                country.Adj = adjective;
                _countries[segments2[0]] = country;
                Console.WriteLine(segments2[0]);
            }
            else
            {
                string name = segments[1].Replace("\"","");
                Country country = new Country()
                {
                    Name = name,
                    Tag = segments2[0]
                };
                _countries.Add(segments2[0],country);
                _tags.Add(segments2[0]);
                Console.WriteLine(name);
            }
        }

        string[] tierDefenitions = File.ReadAllLines("./Input/TierList.txt");
        Tier state = Tier.Empire;
        foreach (string line in tierDefenitions)
        {
            if(line == "")
                continue;
            else if (line.Contains("Empires"))
                state = Tier.Empire;
            else if (line.Contains("Hegemony"))
                state = Tier.Hegemony;
            else if (line.Contains("Kingdoms"))
                state = Tier.Kingdom;
            else if (line.Contains("Grand Principalities"))
                state = Tier.GrandPrincipality;
            else if (line.Contains("Principalities"))
                state = Tier.Principality;
            else if (line.Contains("City-States"))
                state = Tier.CityState;
            else
            {
                Country country = _countries[line.Replace("\n", "")];
                country.Tier = state;
            }
        }

        #if DEBUGMODE
        foreach (string tag in _tags)
        {
            _countries[tag].PrintCountry();
        }
        #endif
        
        _namesFile.Clear();
        _namesFile.Add("l_english:");
        
        Tier[] filter =
        {
            #if EMPIRE
            tier.Empire,
            #endif
            #if HEGEMONY
            tier.Hegemony,
            #endif
            #if KINGDOM
            Tier.Kingdom,
            #endif
            #if GRANDPRINCIPALITY
            Tier.GrandPrincipality,
            #endif
            #if PRINCIPALITY
            Tier.Principality,
            #endif
            #if CITYSTATE
            tier.CityState
            #endif
        };
        
        foreach (String tag in _tags)
        {
            if (filter.Contains(_countries[tag].Tier))
                continue;
            
            _logicFile.AddRange(CreateLogic(_countries[tag]));
            _namesFile.AddRange(CreateLocalization(_countries[tag]));
        }
        
        
        System.IO.File.WriteAllLines("./Output/countries_l_english.yml",_namesFile);
        System.IO.File.WriteAllLines("./Output/00_dynamic_country_names.txt",_logicFile);

        Console.WriteLine("---Completed---");
    }

    private static string[] CreateLocalization(Country country)
    {
        List<String> lines = File.ReadAllLines("./Input/TemplateLocalization.txt").ToList();

        for (int i = 0; i < lines.Count; i++)
        {
            lines[i] = lines[i].Replace("AAA", country.Tag);
            lines[i] = lines[i].Replace("NAME", country.Name);
            lines[i] = lines[i].Replace("ADJE", country.Adj);
            Console.WriteLine(lines[i]);
        }

        return lines.ToArray();
    }

    static string[] CreateLogic(Country country)
    {

        List<String> lines = File.ReadAllLines("./Input/TemplateLogic" + country.Tier + ".txt").ToList();

        for (int i = 0; i < lines.Count; i++)
        {
            lines[i] = lines[i].Replace("AAA", country.Tag);
            lines[i] = lines[i].Replace("NAME", country.Name);
        }
        
        Console.WriteLine("Created logic for tag: " + country.Tag);

        return lines.ToArray();
    }
}