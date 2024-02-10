// Baseball.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <random>

int MarginOfError();
int MarginOfErrorWithSkill(int skillValue);
int calculateWithError(int pitch);
int calculateWithErrorWithSkill(int pitch, int skill);
int setSkillValue(int value);
bool inputCheck(int number);
void calculateResult(int swingResult, int pitchResult);

struct Object{
    bool reset;
    int input;
};

enum Skill {
    novice,
    intermediate,
    expert,
};

int main()
{
    Object Batter{true, 0 };
    Object Machine{true, 0};
    std::string skill;
    while (Machine.reset) // Validation check for inputing the value for the pitching machine
    {
        std::cout << "Pitch Setting(0 to 100): \n";
        std::cin >> Machine.input;
        Machine.reset = inputCheck(Machine.input);
    }
    while (Batter.reset) // Validation check for inputing the value for the batter
    {
        std::cout << "Batter's skill level (Novice, Intermediate, Expert) \n";
        std::cin >> skill;
        if (skill == "novice" || skill == "Novice") { // Sets the batter's skill level based on the inputed string
            Batter.reset = false; // continues the program once the reset boolean is false.
            Batter.input = setSkillValue(novice);  // In this case the skill value is the enum "novice" or 0.
        }
        else if (skill == "intermediate" || skill == "Intermediate") {
            Batter.input = setSkillValue(intermediate);
            Batter.reset = false;
        }
        else if (skill == "expert" || skill == "Expert") {
            Batter.input = setSkillValue(expert);
            Batter.reset = false;
        }
        else {
            std::cout << "Invalid skill level. Try again. \n"; //loops back to the beginning of the loop if previous input was invalid.
            Batter.reset = true;
        }
    }
    int pResult = calculateWithError(Machine.input);
    std::cout << "Here comes the pitch at: " << pResult << "\n";
    std::cout << "And the batter swings! \n";
    int sResult = calculateWithErrorWithSkill(pResult, Batter.input);
    std::cout << "They aimed at... " << sResult << "! \n";
    calculateResult(sResult, pResult);
}

int MarginOfError() //Random number generator that produces a number within range between 0 and 10. Symbolises the deviation of the pitching machine
{
    std::random_device device;
    unsigned seed = device();
    std::default_random_engine engine(seed);
    std::uniform_int_distribution<int>distr(0, 10);
    int error = distr(engine);
    return error;
}

int MarginOfErrorWithSkill(int skillValue) //Similar to the MarginOfError function, but skillValue variable dictates the max of the range.
{
    std::random_device device;
    unsigned seed = device();
    std::default_random_engine engine(seed);
    std::uniform_int_distribution<int>distr(0, skillValue);
    int error = distr(engine);
    return error;
}

int calculateWithError(int pitch) //adds deviation to the pitch
{
    pitch = MarginOfError() + pitch;
    return pitch;
}

int calculateWithErrorWithSkill(int pitch, int skill) //adds deviation to the batter's swing
{
    pitch = MarginOfErrorWithSkill(skill) + pitch;
    return pitch;
}

bool inputCheck(int number) // Validation check for int inputs
{
    bool reset;
    if (std::cin.fail())
    {
        std::cin.clear();
        std::cin.ignore(std::numeric_limits<std::streamsize>::max(), '\n');
        std::cout << "Not a valid setting. Try again \n";
        reset = true;
    }
    else if (number >= 101 || number <= 0 ) {
        std::cout << "Not a valid setting. Try again \n";
        reset = true;
    }else
        reset = false;
    return reset;
}

int setSkillValue(int value) // Sets the range of the margin of error based on the skill value.
{
    int skillValue = 0;
    switch (value)
    {
    case 0:
        skillValue = 10;
        break;
    case 1:
        skillValue = 5;
        break;
    case 2:
        skillValue = 3;
        break;
    }

    return skillValue;
}

void calculateResult(int swingResult, int pitchResult) // Caluculates the result of the hit based on the difference between the pitch value and the swing value
{
    int hit;
    std::string result;
    int diff = std::abs(pitchResult - swingResult);
    if (diff <= 2) 
    {
        hit = 0;
    }
    else if (diff <= 5 && diff >= 2) 
    {
        hit = 1;
    } else 
    {
        hit = 2;
    }

    switch (hit) {
    case 0:
        result = "Homerun!";
        break;
    case 1:
            result = "A clean hit.";
            break;
    case 2:
           result = "Dang...missed.";
           break;
    }
    std::cout << result;
}

