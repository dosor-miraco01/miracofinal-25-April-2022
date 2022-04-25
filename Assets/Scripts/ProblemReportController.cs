using Doozy.Engine.UI;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProblemReportController : MonoBehaviour
{
    public static List<ProblemSaveItem> Saved = new List<ProblemSaveItem>();
    public const string SAVED_KEY = "PROBLEMS_SAVED";

    public ProblemsData problemsData;

    public GameObject problemReportItemPRefab;
    public GameObject problemCaseReportItemPrefab;
    public Transform problemReportParent;
    public UIView reportView;
    public bool openViewAtStart = true;

    private Dictionary<int, List<ProblemCaseReportItemView>> _dic = new Dictionary<int, List<ProblemCaseReportItemView>>();

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey(SAVED_KEY))
        {
            string str = PlayerPrefs.GetString(SAVED_KEY);
            try
            {
                Saved = JsonConvert.DeserializeObject<List<ProblemSaveItem>>(str);
            }
            catch
            {
                Saved = new List<ProblemSaveItem>();
            }
            if (Saved == null) Saved = new List<ProblemSaveItem>();
        }
        GameObject gb = null;
        GameObject gb2 = null;
        ProblemReportItemView priView = null;
        ProblemCaseReportItemView prcView = null;
        foreach (var p in problemsData.items)
        {
            gb = GameObject.Instantiate(problemReportItemPRefab);
            gb.transform.SetParent(problemReportParent);
            _dic[p.problemId] = new List<ProblemCaseReportItemView>();
            priView = gb.GetComponent<ProblemReportItemView>();
            priView.problem = p;
            priView.iconImage.sprite = p.problemReportTitle;
            priView.logicSceneName = problemsData.logicSceneName;
            if (!p.isTitleOnly)
            {
                for (int i = 0; i < p.casesCount; ++i)
                {
                    gb2 = GameObject.Instantiate(problemCaseReportItemPrefab);
                    gb2.transform.SetParent(priView.casesParent);
                    prcView = gb2.GetComponent<ProblemCaseReportItemView>();
                    prcView.text.text = (i + 1).ToString();
                    prcView.problem = p;
                    prcView.logicSceneName = problemsData.logicSceneName;
                    prcView.problemCaseIndex = (i + 1);
                    _dic[p.problemId].Add(prcView);
                    gb2.SetActive(true);
                }
            }
            gb.SetActive(true);
        }
        if (openViewAtStart)
        {
            OpenView();
        }
    }// method


	public static ProblemSaveItem GetProblem(int pid, int caseIndex) {
		if (Saved == null) return new ProblemSaveItem ();
		for(int i = 0; i < Saved.Count; ++i) {
			if (Saved[i].ProblemId == pid && caseIndex == Saved[i].CaseIndex) return Saved[i];
		}
		return new ProblemSaveItem ();
	}
	
    public void OpenView()
    {
        foreach(int pid in _dic.Keys)
        {   
            for (int i = 0; i < _dic[pid].Count; ++i)
            {
                var cd = _dic[pid][i];
                var svd = GetProblem(pid, i + 1);//Saved.SingleOrDefault(x => x.ProblemId == pid && i + 1 == x.CaseIndex);
                if (svd.ProblemId == pid)
                {
                    cd.toggle.isOn = true;
                }
                else
                {
                    cd.toggle.isOn = false;
                }
            }
        }
        reportView.Show();
    }// method

    public static void AddSaveItem(ProblemSaveItem item)
    {
        Saved.Add(item);
        PlayerPrefs.SetString(SAVED_KEY, JsonConvert.SerializeObject(Saved));
    }// method

    public void CloseView()
    {
        reportView.Hide();
    }// method
}
