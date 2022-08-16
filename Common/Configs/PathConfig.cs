using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathConfig
{
    public static string labelPath = "Model/Lessons/LabelManager_1";
    public static string GROUP_INVERSE = "Sprites/Experience/GroupInverse";

    // clicked
    public const string LABEL_CLICKED_IMAGE = "Sprites/Experience/Clicked/Label";
    public const string SEPARATE_CLICKED_IMAGE = "Sprites/Experience/Clicked/Separate";
    public const string XRAY_CLICKED_IMAGE = "Sprites/Experience/Clicked/Xray";
    public const string HOLD_CLICKED_IMAGE = "Sprites/Experience/Clicked/Hold";
    public const string GUIDE_CLICKED_IMAGE = "Sprites/Experience/Clicked/Guide";
    public const string MENU_CLICKED_IMAGE = "Sprites/Experience/Clicked/Menu";
    public const string VIDEO_ICON = "Sprites/Experience/Video";
    public const string MICRO_ON_IMAGE = "Sprites/Experience/Clicked/MicroOn";

    // unclick
    public const string LABEL_UNCLICK_IMAGE = "Sprites/Experience/Unclick/Label";
    public const string SEPARATE_UNCLICK_IMAGE = "Sprites/Experience/Unclick/Separate";
    public const string XRAY_UNCLICK_IMAGE = "Sprites/Experience/Unclick/Xray";
    public const string HOLD_UNCLICK_IMAGE = "Sprites/Experience/Unclick/Hold";
    public const string GUIDE_UNCLICK_IMAGE = "Sprites/Experience/Unclick/Guide";
    public const string MENU_UNCLICK_IMAGE = "Sprites/Experience/Unclick/Menu";
    public const string MICRO_OFF_IMAGE = "Sprites/Experience/UnClick/MicroOff";

    // audio
    public const string AUDIO_PLAY_IMAGE = "Sprites/Experience/Play_black";
    public const string AUDIO_PAUSE_IMAGE = "Sprites/Experience/pause";
    public const string AUDIO_START_PLAY_IMAGE = "Sprites/Experience/startplay";

    // model
    public const string MODEL_PATH = "Model/Experience/Brain_Model2";
    public const string MODEL_ITEM_BODY_TREE = "ItemUI/Experience/ItemBodyTree";
    public const string MODEL_ITEM_HEADER_TREE = "ItemUI/Experience/ItemHeaderTree";
    public const string MODEL_ITEM_MEDIA = "ItemUI/Experience/ItemMedia";
    public const string MODEL_ITEM_NOTIFY = "ItemUI/Meeting/ItemNotification";
    public const string PRE_PATH_MODEL = "Model/Experience/";
    // public const string MODEL_TAG = "ItemUI/Experience/Tag";
    public const string MODEL_TAG = "ItemUI/Experience/LabelLesson";
    public const string MODEL_TAG_LABEL = "ItemUI/Experience/Label";
    public const string MODEL_ADD_LABEL = "ItemUI/Experience/AddLabel";

    public const string FIX_MODEL_TAG = "ItemUI/InteractionModel";
    public const string MODEL_TAG_CONFIG = "ItemUI/Experience/ModelTag";
    public const string MODEL_TAG_EDIT = "ItemUI/Experience/ModelTag_Edit";

    // url video
    public const string VIDEO_PATH = "Assets/Resources/Media/Videos/Experience/";
    public const string MEETING_MEMBER_PATH = "Meetings/MeetingMemberResource";
    public const string MEETING_VOICE_RESOURCE_PATH = "Meetings/MeetingVoiceResource";

    public const string ADD_AUDIO = "ItemUI/Experience/AudioContent";
    public const string ADD_VIDEO = "ItemUI/Experience/VideoContent";
    public static string ORGAN_PANEL_RESOURCE = "ItemUI/itemCategoryWithLesson";
    public static string LESSON_ITEM = "ItemUI/ItemLessonByOrgan";
    public static string LESSON_PANEL_FOR_SEARCH = "ItemUI/Lessons/LessonPanelResource";
    public static string MEDIA_CACHE = Application.persistentDataPath + "/media_cache/";
}
