using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelloFunction
{
    public class FaceAnalysisResult
    {
        public Face[] Faces { get; set; }
        public string ImageId { get; set; }
    }
}
