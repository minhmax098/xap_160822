using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class APIUrlConfig
{
    public static string DOMAIN_SERVER = "https://api.xrcommunity.org/v1/xap/";
    public static string JSON_CONTENT_TYPE_VALUE = "application/json";
    public static string STREAM_CONTENT_TYPE_VALUE = "application/octet-stream";
    public static string CONTENT_TYPE_KEY = "Content-Type";
    public static string AUTHORIZATION_KEY = "Authorization";
    public static string NETWORK_ERROR_MESSAGE = "Can not connect to server, please try again";
    public static int SUCCESS_RESPONSE_CODE = 200;
    public static int BAD_REQUEST_RESPONSE_CODE = 400;
    public static int SERVER_ERROR_RESPONSE_CODE = 500;
    public static int TOKEN_TIME_OUT_CODE = 401;
    public static int SYSTEM_ERROR_CODE = -1;
    public static int DEFAULT_OFFSET = 0;
    public static int DEFAULT_LIMIT = 6;
    public static string GET_ORGAN_LIST = "organs/getListOrgans";
    public static string GET_LESSON_BY_ORGAN = "organs/getListLessonByOrgan?organId={0}&offset={1}&limit={2}";
    public static string POST_CREATE_MEETING_ROOM = "meetings/createRoom ";
    public static string POST_JOIN_MEETING_ROOM = "meetings/joinRoom ";
    public static string GET_LESSON_BY_ID = "lessons/getLessonDetail/{0}";
    public static string GET_SEARCH_LESSONS = "organs/getListLessonByOrgan?searchValue={0}&offset={1}&limit={2}";
    public static string GET_SEARCH_LESSONS_BY_ORGAN = "organs/getListLessonByOrgan?searchValue={0}&offset={1}&limit={2}&organId={3}";
    public static string GET_MY_LESSONS = "lessons/getListMyLesson?searchValue={0}&offset={1}&limit={2}";
    public static string GET_MODEL_LIST = "models/getList3DModel?searchValue={0}&offset={1}&limit={2}&type={3}";
    public static string POST_SIGN_IN = "user/login";
    public static string POST_SIGN_UP = "user/signup";
    public static string GET_ORGAN_WITH_LESSONS = "organs/getListXRLibrary";
    
    public static string POST_CREATE_MODEL_LABEL = "labels/createModelLabel";
    public static string POST_FORGOT_PASS = "user/forgotPassword";
    public static string POST_RESET_PASS = "user/resetPassword";

    public static string POST_CREATE_LESSON_INFO = "lessons/createLessonInfo";
    public static string PATCH_UPDATE_LESSON_INFO = "lessons/updateLessonInfo/{0}";
    public static string PATCH_METHOD = "PATCH";

    public static string POST_ADD_AUDIO_LESSON = "lessons/addAudioLesson";
    public static string POST_ADD_VIDEO_LESSON = "lessons/addVideoLesson";
    public static string POST_ADD_AUDIO_LABEL = "labels/addAudioLabel";
    public static string POST_ADD_VIDEO_LABEL = "labels/addVideoLabel";

    public static string DELETE_AUDIO_LESSON = "lessons/deleteAudioLesson/{0}";
    public static string DELETE_VIDEO_LESSON = "lessons/deleteVideoLesson/{0}";
    public static string DELETE_LABEL = "labels/deleteLabel/{0}";
    public static string DELETE_AUDIO_LABEL = "lessons/deleteAudioLabel/{0}";
    public static string DELETE_VIDEO_LABEL = "lessons/deleteVideoLabel/{0}";


    // API public 
    public static string SignIn = "https://api.xrcommunity.org/v1/xap/user/login";
    public static string SignUp = "https://api.xrcommunity.org/v1/xap/user/signup";
    public static string ForgotPass = "https://api.xrcommunity.org/v1/xap/user/forgotPassword";
    public static string ResetPass = "https://api.xrcommunity.org/v1/xap/user/resetPassword";
    public static string GetListLessonByCategory = "https://api.xrcommunity.org/api/lessons/listByCategoryId?categoryId={0}";
    public static string LoadLesson = "https://api.xrcommunity.org/v1/xap/{0}";
    public static string GetCategoryWithLesson = "https://api.xrcommunity.org/v1/xap/organs/getListXRLibrary";
    public static string GetListLessons = "https://api.xrcommunity.org/v1/xap/organs/getListLessonByOrgan?organId=&searchValue={0}&offset={1}&limit={2}";
    public static string GetListLessonsByOrgan = "https://api.xrcommunity.org/v1/xap/organs/getListLessonByOrgan?organId={0}&searchValue={1}&offset={2}&limit={3}";
    public static string GetLessonsByID = "https://api.xrcommunity.org/v1/xap/lessons/getLessonDetail/{0}";

    public static string GetListMyLesson = "https://api.xrcommunity.org/v1/xap/lessons/getListMyLesson?limit={0}&offset={1}&searchValue={2}";
    public static string GetListOrgans = "https://api.xrcommunity.org/v1/xap/organs/getListOrgans";
    public static string GetList3DModel = "https://api.xrcommunity.org/v1/xap/models/getList3DModel?type={0}&searchValue={1}&offset={2}&limit={3}";

    public static string UpdateLessonInfo = "https://api.xrcommunity.org/v1/xap/lessons/updateLessonInfo/{0}";
    public static string CreateLessonInfo = "https://api.xrcommunity.org/v1/xap/lessons/createLessonInfo";
    public static string Get3DModelDetail = "https://api.xrcommunity.org/v1/xap/models/get3DModelDetail/{0}";

    public static string Upload3DModel = "stores/upload3DModel";
    public static string Import3DModel = "https://api.xrcommunity.org/v1/xap/models/import3DModel";
    public static string CreateModelLabel = "https://api.xrcommunity.org/v1/xap/labels/createModelLabel";

    public static string AddAudioLesson = "https://api.xrcommunity.org/v1/xap/lessons/addAudioLesson";
    public static string AddVideoLesson = "https://api.xrcommunity.org/v1/xap/lessons/addVideoLesson";
    public static string AddAudioLabel = "https://api.xrcommunity.org/v1/xap/labels/addAudioLabel";
    public static string AddVideoLabel = "https://api.xrcommunity.org/v1/xap/labels/addVideoLabel";

    public static string GetLinkVideo = "https://www.youtube.com/oembed?url={0}&format=json";
    public static string GetLinkAPIVideo = "https://www.googleapis.com/youtube/v3/videos?id={0}&key={1}&part=statistics";
    public static string DeleteAudioLesson = "https://api.xrcommunity.org/v1/xap/lessons/deleteAudioLesson/{0}";
    public static string DeleteVideoLesson = "https://api.xrcommunity.org/v1/xap/lessons/deleteVideoLesson/{0}";
    public static string DeleteAudioLabel = "https://api.xrcommunity.org/v1/xap/labels/deleteAudioLabel/{0}";
    public static string DeleteVideoLabel = "https://api.xrcommunity.org/v1/xap/labels/deleteVideoLabel/{0}";
    public static string DeleteLabel = "https://api.xrcommunity.org/v1/xap/labels/deleteLabel/{0}";

    public static string BASE_URL = "https://api.xrcommunity.org/v1/xap/";

    // API private 
    // public static string SignIn = "http://10.60.156.166:8556/v1/xap/user/login";
    // public static string SignUp = "http://10.60.156.166:8556/v1/xap/user/signup";
    // public static string ForgotPass = "http://10.60.156.166:8556/v1/xap/user/forgotPassword";
    // public static string ResetPass = "http://10.60.156.166:8556/v1/xap/user/resetPassword";
    // public static string GetListLessonByCategory = "http://10.60.156.166:8556/v1/xap/lessons/listByCategoryId?categoryId={0}";
    // public static string LoadLesson = "http://10.60.156.166:8556/v1/xap/{0}"; 
    // public static string GetCategoryWithLesson = "http://10.60.156.166:8556/v1/xap/organs/getListXRLibrary"; 
    // public static string GetListLessons = "http://10.60.156.166:8556/v1/xap/organs/getListLessonByOrgan?organId=&searchValue={0}&offset={1}&limit={2}"; 
    // public static string GetListLessonsByOrgan = "http://10.60.156.166:8556/v1/xap/organs/getListLessonByOrgan?organId={0}&searchValue={1}&offset={2}&limit={3}"; 
    // public static string GetLessonsByID = "http://10.60.156.166:8556/v1/xap/lessons/getLessonDetail/{0}"; 
    // public static string GetListMyLesson = "http://10.60.156.166:8556/v1/xap/lessons/getListMyLesson?limit={0}&offset={1}&searchValue={2}"; 
    // public static string GetListOrgans = "http://10.60.156.166:8556/v1/xap/organs/getListOrgans";
    // public static string GetList3DModel = "http://10.60.156.166:8556/v1/xap/models/getList3DModel?type={0}&searchValue={1}&offset={2}&limit={3}";
    // public static string UpdateLessonInfo = "http://10.60.156.166:8556/v1/xap/lessons/updateLessonInfo/{0}";
    // public static string CreateLessonInfo = "http://10.60.156.166:8556/v1/xap/lessons/createLessonInfo"; 
    // public static string Get3DModelDetail = "http://10.60.156.166:8556/v1/xap/models/get3DModelDetail/{0}";
    // public static string Upload3DModel = "http://10.60.156.166:8556/v1/xap/stores/upload3DModel";
    // public static string Import3DModel = "http://10.60.156.166:8556/v1/xap/models/import3DModel";
    // public static string CreateModelLabel = "http://10.60.156.166:8556/v1/xap/labels/createModelLabel";
    // public static string AddAudioLesson = "http://10.60.156.166:8556/v1/xap/lessons/addAudioLesson";
    // public static string AddVideoLesson = "http://10.60.156.166:8556/v1/xap/lessons/addVideoLesson";
    // public static string AddAudioLabel = "http://10.60.156.166:8556/v1/xap/lessons/addAudioLabel";
    // public static string AddVideoLabel = "http://10.60.156.166:8556/v1/xap/lessons/addVideoLabel";
    // public static string GetLinkVideo = "https://www.youtube.com/oembed?url={0}&format=json";
    // public static string GetLinkAPIVideo = "https://www.googleapis.com/youtube/v3/videos?id={0}&key={1}&part=statistics";
    // public static string DeleteAudioLesson = "http://10.60.156.166:8556/v1/xap/lessons/deleteAudioLesson/{0}";
    // public static string DeleteVideoLesson = "http://10.60.156.166:8556/v1/xap/lessons/deleteVideoLesson/{0}";
    // public static string DeleteLabel = "http://10.60.156.166:8556/v1/xap/labels/deleteLabel/{0}";
}
