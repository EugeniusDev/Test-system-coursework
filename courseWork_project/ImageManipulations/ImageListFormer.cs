using System.Collections.Generic;
using System.IO;
using static courseWork_project.ImageManager;

namespace courseWork_project
{
    /// <summary>
    /// Class used to form a list of image infos from database-directory
    /// </summary>
    public class ImageListFormer
    {
        /// <summary>
        /// Gets a list of image infos for specified test
        /// </summary>
        /// <param name="testTitle">Title of test to search linked to images</param>
        /// <param name="questionsList">List of question structures</param>
        /// <returns>List<ImageManager.ImageInfo> for specified test</returns>
        public List<ImageMetadata> GetImageList(string testTitle, List<TestStructs.QuestionMetadata> questionsList)
        {
            List<ImageMetadata> imagesToReturn = new List<ImageMetadata>();

            (string[], bool) allImagesTuple = GetAllImages();
            if (!allImagesTuple.Item2) return imagesToReturn;

            string transliteratedTestTitle = DataDecoder.TransliterateToEnglish(testTitle);
            foreach (string currentImageTitle in allImagesTuple.Item1)
            {
                if (currentImageTitle.Contains(transliteratedTestTitle))
                {
                    string[] splitTitle = currentImageTitle.Split('-');

                    string relativePath = currentImageTitle;
                    string absolutePath = Path.GetFullPath(relativePath);

                    ImageMetadata currImageInfo = new ImageMetadata
                    {
                        imagePath = absolutePath
                    };
                    string betweenNameAndExtension = splitTitle[splitTitle.Length - 1].Split('.')[0];
                    // If image and link to it exists
                    bool imageAndQuestionExist = int.TryParse(betweenNameAndExtension, out currImageInfo.questionIndex)
                        && questionsList.Count >= currImageInfo.questionIndex;
                    if (imageAndQuestionExist 
                        && questionsList[currImageInfo.questionIndex - 1].hasLinkedImage)
                    {
                        imagesToReturn.Add(currImageInfo);
                    }
                }
            }

            return imagesToReturn;
        }
    }
}
