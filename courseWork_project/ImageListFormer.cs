using System.Collections.Generic;
using System.IO;
using static courseWork_project.ImageManager;

namespace courseWork_project
{
    /// <summary>
    /// Клас для формування списку (List) інформації про картинки, що знаходяться в директорії-базі даних
    /// </summary>
    public class ImageListFormer
    {
        /// <summary>
        /// Формує список (List) інформації про картинки, що знаходяться в директорії-базі даних
        /// </summary>
        /// <param name="testTitle">Назва тесту, для якого шукати картинки</param>
        /// <param name="questionsList">Список (List) структур даних запитань тесту</param>
        /// <returns>List<ImageManager.ImageInfo> для подальшої маніпуляції</returns>
        public List<ImageInfo> FormImageList(string testTitle, List<Test.Question> questionsList)
        {
            List<ImageInfo> imagesToReturn = new List<ImageInfo>();

            (string[], bool) allImagesTuple = GetAllImages();
            // Зупинка функції в разі відсутності картинок
            if (!allImagesTuple.Item2) return imagesToReturn;

            string transliteratedTestTitle = DataDecoder.TransliterateAString(testTitle);
            foreach (string currentImageTitle in allImagesTuple.Item1)
            {
                if (currentImageTitle.Contains(transliteratedTestTitle))
                {
                    string[] splitTitle = currentImageTitle.Split(new char[] { '-' });

                    string relativePath = currentImageTitle;
                    string absolutePath = Path.GetFullPath(relativePath);

                    ImageInfo currImageInfo = new ImageInfo();
                    currImageInfo.imagePath = absolutePath;
                    string betweenNameAndExtension = splitTitle[splitTitle.Length - 1].Split(new char[] { '.' })[0];
                    // Якщо картинка та прив'язка до неї існують
                    bool imageAndQuestionExist = int.TryParse(betweenNameAndExtension, out currImageInfo.questionIndex)
                        && questionsList.Count >= currImageInfo.questionIndex;
                    if (imageAndQuestionExist)
                    {
                        // Якщо картинка з даним індексом позначена як прив'язана
                        if (questionsList[currImageInfo.questionIndex - 1].hasLinkedImage)
                        {
                            imagesToReturn.Add(currImageInfo);
                        }
                    }
                }
            }
            return imagesToReturn;
        }
    }
}
