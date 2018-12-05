using System.Collections;
using System.Collections.Generic;

//this class is a singleton class in order to use the  System.Random whith exacle the same seed
public class RandomGenerator
{

    public static int seed=7;// the seed of the ramdom generator 
    private static RandomGenerator instance;//an static instance as the singleton pattern derermines

    public static RandomGenerator Instance //a static methodo to get the instance as the singleton pattern derermines
    {
        get {
                if (instance == null)
                {
                    instance = new RandomGenerator();//initialize the instance
                }
                return instance;
          }
    }

    private System.Random generator;

    private RandomGenerator()//private constructor as the singleton pattern derermines
    {
        generator = new System.Random(seed);
    }

    public int getRamdom(int min, int max)//the function whose returns an ramdom number
    {
        return generator.Next(min, max);
    }
}
