Shiro Programming language.  See http://www.shirodev.com/ for more information.

TL;DR?  Shiro is a reasonably fast and very flexible language with a built-in HTTP server along the node.js lines.  It supports all sorts of unique things like dynamic code injection, parameterized closures, etc.  Making a web service that hooks up to a database is about as easy as:

```
use Http
use SQL

db = sqlOpen("data source=.;initial catalog=Junk;integrated security=True")
Services = {
  end: def(q) { httpStop() },
  echo: def(q) { return q },
  getUser: def(q) {
    rows = sqlQuery(db, "select * from People where Id=1")
    rows = applyToAll(rows, def(i){ 
      return merge(i, IJsonable) 
    })
    return rows[0].toJson()
  }
}

httpStartMapToObject(8081, Services)
```

Shiro is a WIP and is looking for contributors.  Especially if your have the IDE bug like I have the interpreter bug.  While Shiro has a fairly modern IDE, it needs some love and it's not my favorite area to work in.

**Cool Language Feature of the <insert time period here>**

The snippet below is my favorite language-test feature.  It's exercising the 'is' operator (I have a similar snipped for the quack operator ( =? ) as well), and a little bit of the now-famous exec-op to do some dynamic syntax injection.  Now that I look at it I don't think the exec-op is necessary anymore since the type names have become variables instead of keywords, but I love that little squiggly, so I left it in.

```
def Types() {
	n = 123
	s = "bob"
	o = {name: "dan"}
	l = {1,2,3}
	b = true

	f = def() {    print "Hello world"    }
	vals = {n, s, o, l, b, f}
	types = {Num, String, Object, List, Bool, Function}
	notTypes = {String, List, String, Object, Num, Object}
	forAllIdx(vals, def(list, i) {
		checkTrue = types[i] checkFalse = notTypes[i]
		assert.isTrue(list[i] is ~checkTrue, 'is check ' + types[i])
		assert.isFalse(list[i] is ~checkFalse, 'is check (false) ' + types[i])
	})
}
```