using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public static class StaticLesson
{
    public static string LessonTitle { get; set; }
    public static string LessonThumbnail { get; set; }
    public static int Viewed { get; set; }
    public static string LessonObjectives { get; set; }
    public static int LessonId { get; set; }
    public static string Size { get; set; }
    public static int CreatedBy { get; set; }
    public static string AuthorAvatar { get; set; }
    public static string AuthorName { get; set; }
    public static string CreatedDate { get; set; }
    public static int OrganId { get; set; }
    public static int ModelId { get; set; }
    public static string ModelFile { get; set; }
    public static string Video { get; set; }
    public static string Audio { get; set; }
    public static Label[] ListLabel { get; set; }

    public static void SetValueForStaticLesson(LessonDetail _lessonDetail)
    {
        LessonTitle = _lessonDetail.lessonTitle;
        LessonThumbnail = _lessonDetail.lessonThumbnail;
        Viewed = _lessonDetail.viewed;
        LessonObjectives = _lessonDetail.lessonObjectives;
        LessonId = _lessonDetail.lessonId;
        Size = _lessonDetail.size;
        CreatedBy = _lessonDetail.createdBy;
        AuthorAvatar = _lessonDetail.authorAvatar;
        AuthorName = _lessonDetail.authorName;
        CreatedDate = _lessonDetail.createdDate;
        OrganId = _lessonDetail.organId;
        ModelId = _lessonDetail.modelId;
        ModelFile = _lessonDetail.modelFile;
        Video = _lessonDetail.video;
        Audio = _lessonDetail.audio;
        ListLabel = _lessonDetail.listLabel;
    }
}
