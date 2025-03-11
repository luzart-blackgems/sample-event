namespace Luzart
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class GameUtil : MonoBehaviour
    {
        public static GameUtil Instance;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void Start()
        {
            if (isActionPerSecond)
            {
                StartCount();
            }
            if (isOnNewDay)
            {
                WaitAFrame(OnNewDay);
            }
        }
        #region ButtonEffect
        public static void ButtonOnClick(Button bt, UnityAction action, bool isAnim = false, string whereAds = null)
        {
            if (isAnim)
            {
                bt.OnClickAnim(action);
            }
            else
            {
                bt.onClick.RemoveAllListeners();
                bt.onClick.AddListener(action);
            }
        }
        #endregion

        #region String Color
        public static string StringColor(string constColor, object value)
        {
            return $"<color={constColor}>{value}</color>";
        }
        #endregion

        #region Count Per Second
        [SerializeField]
        private bool isActionPerSecond = true;
        private void StartCount()
        {
            StartCoroutine(IECountTime());
        }
        private int timePing = 0;
        private int dayCache = int.MinValue;
        private IEnumerator IECountTime()
        {
            dayCache = DateTimeOffset.FromUnixTimeSeconds(TimeUtils.GetLongTimeCurrent).Day;
            WaitForSeconds wait = new WaitForSeconds(1);
            while (true)
            {
                long timeCb = TimeUtils.GetLongTimeCurrent;
                Observer.Instance.Notify(ObserverKey.TimeActionPerSecond, timeCb);
                timePing++;
                if (timePing >= 5)
                {
                    StartDayCount(timeCb);
                    timePing = 0;
                }
                yield return wait;
            }
        }
        private void StartDayCount(long timeCurrent)
        {
            int dayCacheLocal = DateTimeOffset.FromUnixTimeSeconds(TimeUtils.GetLongTimeCurrent).Day;
            if (dayCacheLocal != dayCache)
            {
                EventManager.Instance.SaveAllData();
                EventManager.Instance.Initialize();
                if (isOnNewDay)
                {
                    WaitAFrame(OnNewDay);
                }
            }
            dayCache = dayCacheLocal;
        }
        #endregion

        #region Coroutine    
        public void WaitAndDo(float time, Action action)
        {
            StartCoroutine(ieWaitAndDo());
            IEnumerator ieWaitAndDo()
            {
                WaitForSeconds wait = new WaitForSeconds(time);
                yield return wait;
                action?.Invoke();
            }
        }
        public void WaitUtilAndDo(Func<bool> predicate, Action action)
        {
            StartCoroutine(ieWaitUtilAndDo());
            IEnumerator ieWaitUtilAndDo()
            {
                WaitUntil wait = new WaitUntil(predicate);
                yield return wait;
                action?.Invoke();
            }
        }
        public void WaitAFrame(Action action)
        {
            StartCoroutine(ieWaitAFrame());
            IEnumerator ieWaitAFrame()
            {
                yield return null;
                action?.Invoke();
            }
        }


        private Dictionary<MonoBehaviour, List<Coroutine>> coroutineDictionary = new Dictionary<MonoBehaviour, List<Coroutine>>();
        public void WaitAndDo(MonoBehaviour behaviour, float time, Action action)
        {
            if (!coroutineDictionary.TryGetValue(behaviour, out var coroutines))
            {
                coroutines = new List<Coroutine>();
                coroutineDictionary[behaviour] = coroutines;
            }

            // Dừng tất cả các coroutine hiện tại của MonoBehaviour (nếu cần thiết)
            StopAllCoroutinesForBehaviour(behaviour);

            Coroutine coroutine = behaviour.StartCoroutine(ieWaitAndDo(time, action));
            coroutines.Add(coroutine);

            IEnumerator ieWaitAndDo(float _time, Action callBack)
            {
                WaitForSeconds wait = new WaitForSeconds(_time);
                yield return wait;
                callBack?.Invoke();
            }
        }
        public void StopAllCoroutinesForBehaviour(MonoBehaviour behaviour)
        {
            if (coroutineDictionary.TryGetValue(behaviour, out var coroutines))
            {
                foreach (var coroutine in coroutines)
                {
                    behaviour.StopCoroutine(coroutine);
                }
                coroutines.Clear();
            }
        }
        #endregion

        #region Coroutine DOVirtual

        public void StartLerpValue(MonoBehaviour behaviour, float preValue, float value, float timeLerp = 1f, Action<float> action = null, Action onComplete = null)
        {
            if (!coroutineDictionary.TryGetValue(behaviour, out var coroutines))
            {
                coroutines = new List<Coroutine>();
                coroutineDictionary[behaviour] = coroutines;
            }
            else
            {
                // Dừng tất cả các coroutine hiện tại của MonoBehaviour
                StopAllCoroutinesForBehaviour(behaviour);
            }

            Coroutine coroutine = behaviour.StartCoroutine(IEFloatLerp(behaviour, preValue, value, timeLerp, action, onComplete));
            coroutines.Add(coroutine);
        }

        private IEnumerator IEFloatLerp(MonoBehaviour behaviour, float preValue, float value, float timeLerp = 1f, Action<float> action = null, Action onComplete = null)
        {
            float timeDeltaTime = Mathf.Approximately(Time.deltaTime, 0) ? (1f / 50f) : Time.deltaTime;
            float sub = timeLerp / timeDeltaTime;
            float delta = (preValue - value) / sub;
            bool isNegativeValue = preValue < value;
            while (true)
            {
                preValue -= delta;
                if ((!isNegativeValue && (preValue <= value)) || (isNegativeValue && (preValue >= value)))
                {
                    action?.Invoke(value);
                    onComplete?.Invoke();
                    coroutineDictionary.Remove(behaviour);
                    yield break;
                }
                action?.Invoke(preValue);
                yield return null;
            }
        }
        #endregion

        #region ONNewDay
        private const string LastCheckedDateKey = "LastCheckedDateSeconds";
        [SerializeField]
        private bool isOnNewDay = true;
        public void OnNewDay()
        {
            // Lấy thời gian hiện tại dưới dạng giây
            long currentSeconds = TimeUtils.GetLongTimeCurrent;

            // Lấy số giây lưu trữ trong PlayerPrefs
            long lastCheckedSeconds = PlayerPrefs.GetInt(LastCheckedDateKey, 0);

            if (lastCheckedSeconds == 0)
            {
                // Nếu không có thời gian lưu trữ trước đó, đây là lần đầu tiên chạy
                PerformNewDayActions(currentSeconds);
            }
            else
            {
                DateTimeOffset lastCheckedDate = DateTimeOffset.FromUnixTimeSeconds(lastCheckedSeconds);
                DateTimeOffset currentDate = DateTimeOffset.FromUnixTimeSeconds(currentSeconds);

                if (currentDate.Date > lastCheckedDate.Date)
                {
                    // Nếu ngày hiện tại khác ngày lưu trữ, thực hiện hành động cho ngày mới
                    PerformNewDayActions(currentSeconds);
                }
            }
        }

        void PerformNewDayActions(long currentSeconds)
        {
            // Cập nhật ngày mới vào PlayerPrefs
            PlayerPrefs.SetInt(LastCheckedDateKey, (int)currentSeconds);
            PlayerPrefs.Save();

            Observer.Instance.Notify(ObserverKey.OnNewDay);

        }
        #endregion

        #region Time And Ordinal To String
        public static string LongTimeSecondToUnixTime(long unixTimeSeconds, bool isDoubleParam = false, string day = "d", string hour = "h", string minutes = "m", string second = "s")
        {
            TimeSpan dateTime = TimeSpan.FromSeconds(unixTimeSeconds);
            return TimeSpanToUnixTime(dateTime, isDoubleParam, day, hour, minutes, second);
        }
        public static string FloatTimeSecondToUnixTime(float unixTimeSeconds, bool isDoubleParam = false, string day = "d", string hour = "h", string minutes = "m", string second = "s")
        {
            TimeSpan dateTime = TimeSpan.FromSeconds(unixTimeSeconds);
            string strValue = TimeSpanToUnixTime(dateTime, isDoubleParam, day, hour, minutes, second);
            int milliseconds = dateTime.Milliseconds;
            if (milliseconds > 0)
            {
                strValue += $".{milliseconds:D3}";
            }
            return strValue;

        }
        public static string TimeSpanToUnixTime(TimeSpan dateTime, bool isDoubleParam = false, string day = "d", string hour = "h", string minutes = "m", string second = "s")
        {
            string strValue = "";
            if (dateTime.Days > 0)
            {
                if (dateTime.Hours == 0 && !isDoubleParam)
                {
                    strValue = $"{dateTime.Days:D2}{day}";
                }
                else
                {
                    strValue = $"{dateTime.Days:D2}{day}:{dateTime.Hours:D2}{hour}";
                }
            }

            else if (dateTime.Hours > 0)
            {
                if (dateTime.Minutes == 0 && !isDoubleParam)
                {
                    strValue = $"{dateTime.Hours:D2}{hour}";
                }
                else
                {
                    strValue = $"{dateTime.Hours:D2}{hour}:{dateTime.Minutes:D2}{minutes}";
                }
            }
            else
            {
                if (dateTime.Seconds == 0 && !isDoubleParam)
                {
                    strValue = $"{dateTime.Minutes:D2}{minutes}";
                }
                else
                {
                    strValue = $"{dateTime.Minutes:D2}{minutes}:{dateTime.Seconds:D2}{second}";
                }
            }
            return strValue;
        }
        public static string ToOrdinal(int number)
        {
            return $"{number}{GetOrdinalSuffix(number)}";
        }
        public static string GetOrdinalSuffix(int number)
        {
            if (number <= 0) return "";

            int lastTwoDigits = number % 100;
            int lastDigit = number % 10;

            if (lastTwoDigits >= 11 && lastTwoDigits <= 13)
            {
                return "th";
            }

            switch (lastDigit)
            {
                case 1:
                    return "st";
                case 2:
                    return "nd";
                case 3:
                    return "rd";
                default:
                    return "th";
            }
        }
        #endregion

        #region Step To Step
        public static void StepToStep(Action<Action>[] step)
        {
            ExecuteStep(step, 0);

            void ExecuteStep(Action<Action>[] stepOnDone, int currentIndex)
            {
                if (currentIndex < stepOnDone.Length)
                {
                    stepOnDone[currentIndex](() =>
                    {
                        // Sau khi bước hoàn thành, tiếp tục bước tiếp theo
                        ExecuteStep(stepOnDone, currentIndex + 1);
                    });
                }
            }
        }
        #endregion

        #region Log
        public static void Log(object debug)
        {
#if DEBUG_DA
            Debug.Log($"<color={ConstColor.ColorBlueLight}>DEBUG_DA: {debug}</color>");
#endif
        }
        public static void LogError(object debug)
        {
#if DEBUG_DA
            Debug.LogError($"<color={ConstColor.ColorBlueLight}>DEBUG_DA: {debug}</color>");
#endif
        }
        #endregion

        #region Vector3
        public static bool ApproximateVector3(Vector3 v1, Vector3 v2)
        {
            if (v1.x - v2.x < 0.01f && v1.y - v2.y < 0.01f && v1.z - v2.z < 0.01f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static float[] Vector2ToFloatArray(Vector2 value)
        {
            return new float[]
            {
                value.x,
                value.y,
            };
        }
        public static Vector2 FloatArrayToVector2(float[] value)
        {
            return new Vector2(value[0], value[1]);
        }
        #endregion

        #region LayerMask
        public static bool IsLayerInLayerMask(int layer, LayerMask layerMask)
        {
            return ((layerMask.value & (1 << layer)) != 0);
        }
        #endregion

        #region Extension Check Null
        public static void SetActiveCheckNull(GameObject ob, bool status)
        {
            if (ob == null) return;
            ob.SetActive(status);
        }
        public static void SetSpriteCheckNull(Image im, Sprite sp)
        {
            if (im == null) return;
            im.sprite = sp;
        }
        public static void SetTextCheckNull(TMPro.TMP_Text txt, string str)
        {
            if (txt == null) return;
            txt.text = str;
        }
        #endregion

        #region Claimed Level
        public static int GetCurrentLevel(int currentLevel)
        {
            int level = currentLevel;
            if (level >= 52)
            {
                level = currentLevel % 40;
                if (level <= 52)
                {
                    level += 10;
                }
            }
            return level;
        }
        #endregion

        #region List
        // Kiểm tra 2 list có giống cả từng phần tử
        public static bool AreListsEqual<T>(List<T> list1, List<T> list2)
        {
            var groupedList1 = list1.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
            var groupedList2 = list2.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());

            return groupedList1.Count == groupedList2.Count &&
                   groupedList1.All(pair => groupedList2.ContainsKey(pair.Key) && groupedList2[pair.Key] == pair.Value);
        }
        // Lấy phần tử random trong khoảng từ bao nhiêu đến bao nhiêu
        public static int GetRandomValueInCountStack(int total, int contain, int count, int minGive)
        {
            int valueSub = (total - contain);
            if (count <= 1)
            {
                return valueSub;
            }
            int canSub = valueSub - minGive * (count - 1);
            int valueSure = UnityEngine.Random.Range(minGive, canSub);
            return valueSure;
        }
        // Nêú array quá lớn thì lấy thằng cuối cùng
        public static int GetIndexInArrayInt(int[] array, int a)
        {
            int length = array.Length;
            for (int i = 0; i < length - 1; i++)
            {
                if (a > array[i] && a <= array[i + 1])
                {
                    return i;
                }
            }

            // Nếu a lớn hơn giá trị cuối cùng
            if (a > array[^1])
            {
                return array.Length - 1;
            }

            // Nếu không thỏa mãn điều kiện nào
            return 0; // Hoặc giá trị mặc định khác
        }
        #endregion

        public static List<GameObject> FindGameObjectsByName(string name)
        {
            // Tạo danh sách để lưu các GameObject tìm được
            List<GameObject> objectsWithName = new List<GameObject>();

            // Lấy tất cả các GameObject trong scene
            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

            // Kiểm tra từng GameObject xem có tên là "collidd" không
            foreach (GameObject obj in allObjects)
            {
                if (obj.name == name)
                {
                    objectsWithName.Add(obj);
                }
            }

            return objectsWithName;
        }
    }
    public static class ConstColor
    {
        public const string ColorRed = "#FF0000";
        public const string ColorGreen = "#00FF06";
        public const string ColorBlack = "#000000";
        public const string ColorWhite = "#FFFFFF";
        public const string ColorBlueLight = "#00FFF0";
        public const string ColorBlueDark = "#000FFF";
        public const string ColorYellow = "#FFFF00";
        public const string ColorPurple = "#7300B3";
    }

    public static class UIExtension
    {
        public static void OnClickAnim(this Button btn, UnityAction action)
        {
            if (btn == null)
            {
                return;
            }
            var effect = btn.GetComponent<EffectButton>();
            if (effect == null)
            {
                effect = btn.gameObject.AddComponent<EffectButton>();
            }
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                action?.Invoke();
            });
        }
        public static Vector2 GetRandomPositionInRect(this RectTransform targetRectTransform)
        {
            if (targetRectTransform == null)
            {
                Debug.LogError("targetRectTransform is null!");
                return Vector2.zero;
            }

            // Lấy kích thước của RectTransform
            Vector2 size = targetRectTransform.rect.size;

            // Lấy tọa độ ngẫu nhiên bên trong kích thước
            float randomX = UnityEngine.Random.Range(-size.x / 2, size.x / 2);
            float randomY = UnityEngine.Random.Range(-size.y / 2, size.y / 2);

            // Trả về vị trí ngẫu nhiên trong RectTransform (trong không gian cục bộ)
            return new Vector2(randomX, randomY);
        }
        public static void CallOnEnable(this GameObject obj)
        {
            if (obj == null)
            {
                return;
            }
            obj.SetActive(false);
            obj.SetActive(true);
        }
        public static void ChangeParentAndKeepSize(this RectTransform childRectTransform, RectTransform newParent)
        {
            // Lưu kích thước trong không gian thế giới trước khi đổi cha
            Vector2 worldSizeBefore = GetWorldSize(childRectTransform);

            // Đổi cha cho đối tượng
            childRectTransform.SetParent(newParent, false); // 'false' để không thay đổi vị trí trong không gian thế giới ngay lập tức

            // Tính tỉ lệ của cha mới trong không gian thế giới
            Vector3 newParentWorldScale = newParent.lossyScale;

            // Cập nhật lại sizeDelta dựa trên kích thước thế giới và tỉ lệ cha mới
            childRectTransform.sizeDelta = new Vector2(worldSizeBefore.x / newParentWorldScale.x, worldSizeBefore.y / newParentWorldScale.y);

            Vector2 GetWorldSize(RectTransform rectTransform)
            {
                Vector2 sizeDelta = rectTransform.sizeDelta;
                Vector3 worldScale = rectTransform.lossyScale;

                // Kích thước trong không gian thế giới
                return new Vector2(sizeDelta.x * worldScale.x, sizeDelta.y * worldScale.y);
            }
        }
        public static void ClaimedRectTransformScrollView(this ScrollRect scrollRect, RectTransform itemRectTransform)
        {
            Canvas.ForceUpdateCanvases();

            Vector3[] itemCorners = new Vector3[4];
            itemRectTransform.GetWorldCorners(itemCorners);
            Vector3[] viewCorners = new Vector3[4];
            scrollRect.viewport.GetWorldCorners(viewCorners);

            float difference = 0;

            if (scrollRect.horizontal)
            {
                if (itemCorners[2].x > viewCorners[2].x)
                {
                    difference = itemCorners[2].x - viewCorners[2].x;
                }
                else if (itemCorners[0].x < viewCorners[0].x)
                {
                    difference = itemCorners[0].x - viewCorners[0].x;
                }

                float width = viewCorners[2].x - viewCorners[0].x;
                float normalizedDifference = difference / width;

                Vector2 posCurrent = scrollRect.content.anchoredPosition;
                Vector2 size = scrollRect.content.sizeDelta;

                float newX = posCurrent.x - normalizedDifference * size.x;
                float minX = 0;
                float maxX = scrollRect.content.sizeDelta.x - scrollRect.viewport.rect.width;

                scrollRect.content.anchoredPosition = new Vector2(Mathf.Clamp(newX, minX, maxX), posCurrent.y);
            }
            else
            {
                if (itemCorners[1].y > viewCorners[1].y)
                {
                    difference = itemCorners[1].y - viewCorners[1].y;
                }
                else if (itemCorners[0].y < viewCorners[0].y)
                {
                    difference = itemCorners[0].y - viewCorners[0].y;
                }

                float height = viewCorners[1].y - viewCorners[0].y;
                float normalizedDifference = difference / height;

                Vector2 posCurrent = scrollRect.content.anchoredPosition;
                Vector2 size = scrollRect.content.sizeDelta;

                float newY = posCurrent.y - normalizedDifference * size.y;
                float minY = 0;
                float maxY = scrollRect.content.sizeDelta.y - scrollRect.viewport.rect.height;

                scrollRect.content.anchoredPosition = new Vector2(posCurrent.x, Mathf.Clamp(newY, minY, maxY));
            }
        }

        public static void ClaimedRectTransformScrollView(this ScrollRect scrollRect, RectTransform itemRectTransform, float elasticity)
        {
            scrollRect.elasticity = 0.5f;
            ClaimedRectTransformScrollView(scrollRect, itemRectTransform);
        }

        public static void FocusOnRectTransform(this ScrollRect scrollRect, RectTransform itemRectTransform)
        {
            float contentHeight = scrollRect.content.rect.height;
            float viewportHeight = scrollRect.viewport.rect.height;
            float targetPositionY = 0f;

            // Tính vị trí phần tử dựa trên offset của VerticalLayoutGroup
            for (int i = 0; i < scrollRect.content.childCount; i++)
            {
                RectTransform child = scrollRect.content.GetChild(i) as RectTransform;
                if (child == itemRectTransform)
                {
                    break; // Dừng lại khi đến phần tử cần cuộn tới
                }
                targetPositionY += child.rect.height;
            }

            // Điều chỉnh vị trí mục tiêu để phần tử hiển thị không sát mép trên
            float elementOffset = viewportHeight / 2 - itemRectTransform.rect.height / 2;
            targetPositionY -= elementOffset;

            // Tính giá trị cuộn
            float normalizedPosition = Mathf.Clamp01(1 - (targetPositionY / (contentHeight - viewportHeight)));

            // Cuộn đến phần tử
            scrollRect.verticalNormalizedPosition = normalizedPosition;
        }
        public static void FocusOnRectTransformFromBottom(this ScrollRect scrollRect, RectTransform itemRectTransform)
        {
            float contentHeight = scrollRect.content.rect.height;
            float viewportHeight = scrollRect.viewport.rect.height;
            float targetPositionY = 0f;

            // Tính vị trí phần tử dựa trên offset của VerticalLayoutGroup, nhưng từ dưới lên trên
            for (int i = scrollRect.content.childCount - 1; i >= 0; i--)
            {
                RectTransform child = scrollRect.content.GetChild(i) as RectTransform;
                if (child == itemRectTransform)
                {
                    break; // Dừng lại khi đến phần tử cần cuộn tới
                }
                targetPositionY += child.rect.height;
            }

            // Điều chỉnh vị trí mục tiêu để phần tử hiển thị không sát mép dưới
            float elementOffset = viewportHeight / 2 - itemRectTransform.rect.height / 2;
            targetPositionY -= elementOffset;

            // Tính giá trị cuộn từ dưới lên
            float normalizedPosition = Mathf.Clamp01(targetPositionY / (contentHeight - viewportHeight));

            // Cuộn đến phần tử
            scrollRect.verticalNormalizedPosition = normalizedPosition;
        }

        public static float CalculatePositionInHorizontalScroll(this ScrollRect scrollRect, RectTransform itemRectTransform)
        {
            // Lấy kích thước và vị trí của content trong tọa độ viewport
            var contentBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(scrollRect.viewport, scrollRect.content);
            var itemBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(scrollRect.viewport, itemRectTransform);

            // Tính vị trí mục tiêu của phần tử trong content
            float contentWidth = contentBounds.size.x;
            float viewportWidth = scrollRect.viewport.rect.width;

            // Xác định vị trí mục tiêu
            float targetPositionX = itemBounds.center.x - contentBounds.min.x;
            float elementOffset = viewportWidth / 2 - itemRectTransform.rect.width / 2;
            targetPositionX -= elementOffset;

            // Tính toán giá trị cuộn chuẩn hóa
            float normalizedPosition = Mathf.Clamp01(targetPositionX / (contentWidth - viewportWidth));

            return normalizedPosition;
        }
        public static void SetInteractable(this Button bt, bool interactable)
        {
            var graphics = bt.GetComponentsInChildren<MaskableGraphic>();
            bt.interactable = interactable;
            for (int i = 0; i < graphics.Length; i++)
            {
                graphics[i].color = interactable ? bt.colors.normalColor : bt.colors.disabledColor;
            }
        }

        public static int SumRange(this IList<int> collection, int min, int max)
        {
            int num = 0;
            for (var i = min; i <= max && i < collection.Count; i++)
            {
                int obj = collection[i];
                num += obj;
            }

            return num;
        }

        public static int IndexOf<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            int num = 0;
            foreach (T obj in collection)
            {
                if (predicate(obj))
                    return num;
                ++num;
            }
            return -1;
        }
    }

    // Sinh vat the va tu pool
    public static class MasterHelper
    {
        public static void InitListObj<Tobj, Tdata>(IList<Tdata> data, Tobj objPf, IList<Tobj> objs, Transform holdObj, System.Action<Tobj, int> onSetup) where Tobj : MonoBehaviour
        {
            if (objs == null)
            {
                objs = new List<Tobj>();
            }
            objPf.gameObject.SetActive(false);
            if (data != null)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    Tobj n;
                    var idx = i;
                    if (i < objs.Count)
                    {
                        n = objs[idx];
                    }
                    else
                    {
                        n = UnityEngine.Object.Instantiate(objPf, holdObj);
                        objs.Add(n);
                    }
                    onSetup?.Invoke(n, idx);
                }
            }
            var c = data == null ? 0 : data.Count;
            if (c < objs.Count)
            {
                for (int i = c; i < objs.Count; i++)
                {
                    objs[i].gameObject.SetActive(false);
                }
            }
        }
        public static void InitListObj<Tobj>(int num, Tobj objPf, IList<Tobj> objs, Transform holdObj, System.Action<Tobj, int> onSetup) where Tobj : MonoBehaviour
        {
            if (objs == null)
            {
                objs = new List<Tobj>();
            }
            objPf.gameObject.SetActive(false);
            for (int i = 0; i < num; i++)
            {
                Tobj n;
                var idx = i;
                if (i < objs.Count)
                {
                    n = objs[idx];
                }
                else
                {
                    n = UnityEngine.Object.Instantiate(objPf, holdObj);
                    objs.Add(n);
                }
                onSetup?.Invoke(n, idx);
            }
            if (num < objs.Count)
            {
                for (int i = num; i < objs.Count; i++)
                {
                    objs[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        private static readonly object _lock = new();

        public static T Instance
        {
            get
            {
                if (applicationIsQuitting)
                {
                    Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                    "' already destroyed on application quit." +
                    " Won't create again - returning null.");
                    return null;
                }

                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            Debug.LogError("[Singleton] Something went really wrong " +
                            " - there should never be more than 1 singleton!" +
                            " Reopenning the scene might fix it.");
                            return _instance;
                        }

                        if (_instance == null)
                        {
                            GameObject singleton = new();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = "(singleton)" + typeof(T).ToString();


                            Debug.Log("[Singleton] An instance of " + typeof(T) +
                            " is needed in the scene, so '" + singleton +
                            "' was created with DontDestroyOnLoad.");
                        }
                        else
                        {
                            //Debug.Log("[Singleton] Using instance already created: " +  _instance.gameObject.name);
                        }
                        //DontDestroyOnLoad(_instance.gameObject);
                    }
                }
                return _instance;
            }
        }

        private static bool applicationIsQuitting = false;
        public virtual void OnDestroy()
        {
            applicationIsQuitting = true;
        }
    }
}
