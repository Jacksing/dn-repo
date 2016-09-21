# dn-repo
Useful common .net modules.


#KuaiDiHundred.cs
---
**Create an instance of `KuaiDiHundred` and then implement following three events for it:**

	// You can initialize express number nor express company here.
	var kuaidiHundred = new KuaiDiHundred(...);

	// Raises when the express number can belong to multi express company.
	kuaidiHundred.SelectCompanyHandler += <your callback>;

	// Raises when express status information comes back.
	kuaidiHundred.WriteExpressStatusHandler += <your callback>;

	// Raises if any error happens in module inner.
	kuaidiHundred.ErrorResultHandler += <your callback>;

**Then, call the action method `QueryExpressStatus`:**

	kuaidiHundred.QueryExpressStatus(...)

###やった、お楽しみに！