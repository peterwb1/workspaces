using System.Collections.Generic;

namespace ExerciseApp.Model
{
    public class ModelSpec {
        public string Make { get; set; }
        public List<string> Models { get; private set; } = new();
    }
}


