    using System;

[Serializable]
public class CharacterCard
{
    public string spec;
    public string spec_version;
    public CharacterData data;
}

[Serializable]
public class CharacterData
{
    public string name;
    public string description;
    public string personality;
    public string scenario;
    public string first_mes;
    public string mes_example;

    public string creator_notes;
    public string system_prompt;
    public string post_history_instructions;
    public string[] alternate_greetings;

    //public CharacterBook characterBook;

    public string[] tags;
    public string creator;
    public string character_version;

    // Extensions?
}



//    type TavernCardV2 = {
//  spec: 'chara_card_v2'
//  spec_version: '2.0' // May 8th addition
//  data: {
//    name: string
//    description: string
//    personality: string
//    scenario: string
//    first_mes: string
//    mes_example: string

//    // New fields start here
//    creator_notes: string
//    system_prompt: string
//    post_history_instructions: string
//    alternate_greetings: Array<string>
//    character_book?: CharacterBook

//    // May 8th additions
//    tags: Array<string>
//    creator: string
//    character_version: string
//    extensions: Record<string, any> // see details for explanation
//  }
//}
