package com.miku.utils;

import java.io.UnsupportedEncodingException;
import java.util.ArrayList;
import java.util.Collection;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;

import javax.script.ScriptEngine;
import javax.script.ScriptEngineManager;
import javax.script.ScriptException;

import org.apache.commons.beanutils.PropertyUtils;
import org.apache.commons.lang3.StringUtils;

import com.alibaba.fastjson.util.Base64;

@SuppressWarnings("rawtypes")
public class AESUtils {
	private static String code = "LyoKQ3J5cHRvSlMgdjMuMC4yCmNvZGUuZ29vZ2xlLmNvbS9wL2NyeXB0by1qcwooYykgMjAwOS0yMDEyIGJ5IEplZmYgTW90dC4gQWxsIHJpZ2h0cyByZXNlcnZlZC4KY29kZS5nb29nbGUuY29tL3AvY3J5cHRvLWpzL3dpa2kvTGljZW5zZQoqLwp2YXIgQ3J5cHRvSlM9Q3J5cHRvSlN8fGZ1bmN0aW9uKHAsaCl7dmFyIGk9e30sbD1pLmxpYj17fSxyPWwuQmFzZT1mdW5jdGlvbigpe2Z1bmN0aW9uIGEoKXt9cmV0dXJue2V4dGVuZDpmdW5jdGlvbihlKXthLnByb3RvdHlwZT10aGlzO3ZhciBjPW5ldyBhO2UmJmMubWl4SW4oZSk7Yy4kc3VwZXI9dGhpcztyZXR1cm4gY30sY3JlYXRlOmZ1bmN0aW9uKCl7dmFyIGE9dGhpcy5leHRlbmQoKTthLmluaXQuYXBwbHkoYSxhcmd1bWVudHMpO3JldHVybiBhfSxpbml0OmZ1bmN0aW9uKCl7fSxtaXhJbjpmdW5jdGlvbihhKXtmb3IodmFyIGMgaW4gYSlhLmhhc093blByb3BlcnR5KGMpJiYodGhpc1tjXT1hW2NdKTthLmhhc093blByb3BlcnR5KCJ0b1N0cmluZyIpJiYodGhpcy50b1N0cmluZz1hLnRvU3RyaW5nKX0sY2xvbmU6ZnVuY3Rpb24oKXtyZXR1cm4gdGhpcy4kc3VwZXIuZXh0ZW5kKHRoaXMpfX19KCksbz1sLldvcmRBcnJheT1yLmV4dGVuZCh7aW5pdDpmdW5jdGlvbihhLGUpe2E9CnRoaXMud29yZHM9YXx8W107dGhpcy5zaWdCeXRlcz1lIT1oP2U6NCphLmxlbmd0aH0sdG9TdHJpbmc6ZnVuY3Rpb24oYSl7cmV0dXJuKGF8fHMpLnN0cmluZ2lmeSh0aGlzKX0sY29uY2F0OmZ1bmN0aW9uKGEpe3ZhciBlPXRoaXMud29yZHMsYz1hLndvcmRzLGI9dGhpcy5zaWdCeXRlcyxhPWEuc2lnQnl0ZXM7dGhpcy5jbGFtcCgpO2lmKGIlNClmb3IodmFyIGQ9MDtkPGE7ZCsrKWVbYitkPj4+Ml18PShjW2Q+Pj4yXT4+PjI0LTgqKGQlNCkmMjU1KTw8MjQtOCooKGIrZCklNCk7ZWxzZSBpZig2NTUzNTxjLmxlbmd0aClmb3IoZD0wO2Q8YTtkKz00KWVbYitkPj4+Ml09Y1tkPj4+Ml07ZWxzZSBlLnB1c2guYXBwbHkoZSxjKTt0aGlzLnNpZ0J5dGVzKz1hO3JldHVybiB0aGlzfSxjbGFtcDpmdW5jdGlvbigpe3ZhciBhPXRoaXMud29yZHMsZT10aGlzLnNpZ0J5dGVzO2FbZT4+PjJdJj00Mjk0OTY3Mjk1PDwzMi04KihlJTQpO2EubGVuZ3RoPXAuY2VpbChlLzQpfSxjbG9uZTpmdW5jdGlvbigpe3ZhciBhPQpyLmNsb25lLmNhbGwodGhpcyk7YS53b3Jkcz10aGlzLndvcmRzLnNsaWNlKDApO3JldHVybiBhfSxyYW5kb206ZnVuY3Rpb24oYSl7Zm9yKHZhciBlPVtdLGM9MDtjPGE7Yys9NCllLnB1c2goNDI5NDk2NzI5NipwLnJhbmRvbSgpfDApO3JldHVybiBvLmNyZWF0ZShlLGEpfX0pLG09aS5lbmM9e30scz1tLkhleD17c3RyaW5naWZ5OmZ1bmN0aW9uKGEpe2Zvcih2YXIgZT1hLndvcmRzLGE9YS5zaWdCeXRlcyxjPVtdLGI9MDtiPGE7YisrKXt2YXIgZD1lW2I+Pj4yXT4+PjI0LTgqKGIlNCkmMjU1O2MucHVzaCgoZD4+PjQpLnRvU3RyaW5nKDE2KSk7Yy5wdXNoKChkJjE1KS50b1N0cmluZygxNikpfXJldHVybiBjLmpvaW4oIiIpfSxwYXJzZTpmdW5jdGlvbihhKXtmb3IodmFyIGU9YS5sZW5ndGgsYz1bXSxiPTA7YjxlO2IrPTIpY1tiPj4+M118PXBhcnNlSW50KGEuc3Vic3RyKGIsMiksMTYpPDwyNC00KihiJTgpO3JldHVybiBvLmNyZWF0ZShjLGUvMil9fSxuPW0uTGF0aW4xPXtzdHJpbmdpZnk6ZnVuY3Rpb24oYSl7Zm9yKHZhciBlPQphLndvcmRzLGE9YS5zaWdCeXRlcyxjPVtdLGI9MDtiPGE7YisrKWMucHVzaChTdHJpbmcuZnJvbUNoYXJDb2RlKGVbYj4+PjJdPj4+MjQtOCooYiU0KSYyNTUpKTtyZXR1cm4gYy5qb2luKCIiKX0scGFyc2U6ZnVuY3Rpb24oYSl7Zm9yKHZhciBlPWEubGVuZ3RoLGM9W10sYj0wO2I8ZTtiKyspY1tiPj4+Ml18PShhLmNoYXJDb2RlQXQoYikmMjU1KTw8MjQtOCooYiU0KTtyZXR1cm4gby5jcmVhdGUoYyxlKX19LGs9bS5VdGY4PXtzdHJpbmdpZnk6ZnVuY3Rpb24oYSl7dHJ5e3JldHVybiBkZWNvZGVVUklDb21wb25lbnQoZXNjYXBlKG4uc3RyaW5naWZ5KGEpKSl9Y2F0Y2goZSl7dGhyb3cgRXJyb3IoIk1hbGZvcm1lZCBVVEYtOCBkYXRhIik7fX0scGFyc2U6ZnVuY3Rpb24oYSl7cmV0dXJuIG4ucGFyc2UodW5lc2NhcGUoZW5jb2RlVVJJQ29tcG9uZW50KGEpKSl9fSxmPWwuQnVmZmVyZWRCbG9ja0FsZ29yaXRobT1yLmV4dGVuZCh7cmVzZXQ6ZnVuY3Rpb24oKXt0aGlzLl9kYXRhPW8uY3JlYXRlKCk7CnRoaXMuX25EYXRhQnl0ZXM9MH0sX2FwcGVuZDpmdW5jdGlvbihhKXsic3RyaW5nIj09dHlwZW9mIGEmJihhPWsucGFyc2UoYSkpO3RoaXMuX2RhdGEuY29uY2F0KGEpO3RoaXMuX25EYXRhQnl0ZXMrPWEuc2lnQnl0ZXN9LF9wcm9jZXNzOmZ1bmN0aW9uKGEpe3ZhciBlPXRoaXMuX2RhdGEsYz1lLndvcmRzLGI9ZS5zaWdCeXRlcyxkPXRoaXMuYmxvY2tTaXplLHE9Yi8oNCpkKSxxPWE/cC5jZWlsKHEpOnAubWF4KChxfDApLXRoaXMuX21pbkJ1ZmZlclNpemUsMCksYT1xKmQsYj1wLm1pbig0KmEsYik7aWYoYSl7Zm9yKHZhciBqPTA7ajxhO2orPWQpdGhpcy5fZG9Qcm9jZXNzQmxvY2soYyxqKTtqPWMuc3BsaWNlKDAsYSk7ZS5zaWdCeXRlcy09Yn1yZXR1cm4gby5jcmVhdGUoaixiKX0sY2xvbmU6ZnVuY3Rpb24oKXt2YXIgYT1yLmNsb25lLmNhbGwodGhpcyk7YS5fZGF0YT10aGlzLl9kYXRhLmNsb25lKCk7cmV0dXJuIGF9LF9taW5CdWZmZXJTaXplOjB9KTtsLkhhc2hlcj1mLmV4dGVuZCh7aW5pdDpmdW5jdGlvbigpe3RoaXMucmVzZXQoKX0sCnJlc2V0OmZ1bmN0aW9uKCl7Zi5yZXNldC5jYWxsKHRoaXMpO3RoaXMuX2RvUmVzZXQoKX0sdXBkYXRlOmZ1bmN0aW9uKGEpe3RoaXMuX2FwcGVuZChhKTt0aGlzLl9wcm9jZXNzKCk7cmV0dXJuIHRoaXN9LGZpbmFsaXplOmZ1bmN0aW9uKGEpe2EmJnRoaXMuX2FwcGVuZChhKTt0aGlzLl9kb0ZpbmFsaXplKCk7cmV0dXJuIHRoaXMuX2hhc2h9LGNsb25lOmZ1bmN0aW9uKCl7dmFyIGE9Zi5jbG9uZS5jYWxsKHRoaXMpO2EuX2hhc2g9dGhpcy5faGFzaC5jbG9uZSgpO3JldHVybiBhfSxibG9ja1NpemU6MTYsX2NyZWF0ZUhlbHBlcjpmdW5jdGlvbihhKXtyZXR1cm4gZnVuY3Rpb24oZSxjKXtyZXR1cm4gYS5jcmVhdGUoYykuZmluYWxpemUoZSl9fSxfY3JlYXRlSG1hY0hlbHBlcjpmdW5jdGlvbihhKXtyZXR1cm4gZnVuY3Rpb24oZSxjKXtyZXR1cm4gZy5ITUFDLmNyZWF0ZShhLGMpLmZpbmFsaXplKGUpfX19KTt2YXIgZz1pLmFsZ289e307cmV0dXJuIGl9KE1hdGgpOwooZnVuY3Rpb24oKXt2YXIgcD1DcnlwdG9KUyxoPXAubGliLldvcmRBcnJheTtwLmVuYy5CYXNlNjQ9e3N0cmluZ2lmeTpmdW5jdGlvbihpKXt2YXIgbD1pLndvcmRzLGg9aS5zaWdCeXRlcyxvPXRoaXMuX21hcDtpLmNsYW1wKCk7Zm9yKHZhciBpPVtdLG09MDttPGg7bSs9Mylmb3IodmFyIHM9KGxbbT4+PjJdPj4+MjQtOCoobSU0KSYyNTUpPDwxNnwobFttKzE+Pj4yXT4+PjI0LTgqKChtKzEpJTQpJjI1NSk8PDh8bFttKzI+Pj4yXT4+PjI0LTgqKChtKzIpJTQpJjI1NSxuPTA7ND5uJiZtKzAuNzUqbjxoO24rKylpLnB1c2goby5jaGFyQXQocz4+PjYqKDMtbikmNjMpKTtpZihsPW8uY2hhckF0KDY0KSlmb3IoO2kubGVuZ3RoJTQ7KWkucHVzaChsKTtyZXR1cm4gaS5qb2luKCIiKX0scGFyc2U6ZnVuY3Rpb24oaSl7dmFyIGk9aS5yZXBsYWNlKC9ccy9nLCIiKSxsPWkubGVuZ3RoLHI9dGhpcy5fbWFwLG89ci5jaGFyQXQoNjQpO28mJihvPWkuaW5kZXhPZihvKSwtMSE9byYmKGw9bykpOwpmb3IodmFyIG89W10sbT0wLHM9MDtzPGw7cysrKWlmKHMlNCl7dmFyIG49ci5pbmRleE9mKGkuY2hhckF0KHMtMSkpPDwyKihzJTQpLGs9ci5pbmRleE9mKGkuY2hhckF0KHMpKT4+PjYtMioocyU0KTtvW20+Pj4yXXw9KG58ayk8PDI0LTgqKG0lNCk7bSsrfXJldHVybiBoLmNyZWF0ZShvLG0pfSxfbWFwOiJBQkNERUZHSElKS0xNTk9QUVJTVFVWV1hZWmFiY2RlZmdoaWprbG1ub3BxcnN0dXZ3eHl6MDEyMzQ1Njc4OSsvPSJ9fSkoKTsKKGZ1bmN0aW9uKHApe2Z1bmN0aW9uIGgoZixnLGEsZSxjLGIsZCl7Zj1mKyhnJmF8fmcmZSkrYytkO3JldHVybihmPDxifGY+Pj4zMi1iKStnfWZ1bmN0aW9uIGkoZixnLGEsZSxjLGIsZCl7Zj1mKyhnJmV8YSZ+ZSkrYytkO3JldHVybihmPDxifGY+Pj4zMi1iKStnfWZ1bmN0aW9uIGwoZixnLGEsZSxjLGIsZCl7Zj1mKyhnXmFeZSkrYytkO3JldHVybihmPDxifGY+Pj4zMi1iKStnfWZ1bmN0aW9uIHIoZixnLGEsZSxjLGIsZCl7Zj1mKyhhXihnfH5lKSkrYytkO3JldHVybihmPDxifGY+Pj4zMi1iKStnfXZhciBvPUNyeXB0b0pTLG09by5saWIscz1tLldvcmRBcnJheSxtPW0uSGFzaGVyLG49by5hbGdvLGs9W107KGZ1bmN0aW9uKCl7Zm9yKHZhciBmPTA7NjQ+ZjtmKyspa1tmXT00Mjk0OTY3Mjk2KnAuYWJzKHAuc2luKGYrMSkpfDB9KSgpO249bi5NRDU9bS5leHRlbmQoe19kb1Jlc2V0OmZ1bmN0aW9uKCl7dGhpcy5faGFzaD1zLmNyZWF0ZShbMTczMjU4NDE5Myw0MDIzMjMzNDE3LAoyNTYyMzgzMTAyLDI3MTczMzg3OF0pfSxfZG9Qcm9jZXNzQmxvY2s6ZnVuY3Rpb24oZixnKXtmb3IodmFyIGE9MDsxNj5hO2ErKyl7dmFyIGU9ZythLGM9ZltlXTtmW2VdPShjPDw4fGM+Pj4yNCkmMTY3MTE5MzV8KGM8PDI0fGM+Pj44KSY0Mjc4MjU1MzYwfWZvcih2YXIgZT10aGlzLl9oYXNoLndvcmRzLGM9ZVswXSxiPWVbMV0sZD1lWzJdLHE9ZVszXSxhPTA7NjQ+YTthKz00KTE2PmE/KGM9aChjLGIsZCxxLGZbZythXSw3LGtbYV0pLHE9aChxLGMsYixkLGZbZythKzFdLDEyLGtbYSsxXSksZD1oKGQscSxjLGIsZltnK2ErMl0sMTcsa1thKzJdKSxiPWgoYixkLHEsYyxmW2crYSszXSwyMixrW2ErM10pKTozMj5hPyhjPWkoYyxiLGQscSxmW2crKGErMSklMTZdLDUsa1thXSkscT1pKHEsYyxiLGQsZltnKyhhKzYpJTE2XSw5LGtbYSsxXSksZD1pKGQscSxjLGIsZltnKyhhKzExKSUxNl0sMTQsa1thKzJdKSxiPWkoYixkLHEsYyxmW2crYSUxNl0sMjAsa1thKzNdKSk6NDg+YT8oYz0KbChjLGIsZCxxLGZbZysoMyphKzUpJTE2XSw0LGtbYV0pLHE9bChxLGMsYixkLGZbZysoMyphKzgpJTE2XSwxMSxrW2ErMV0pLGQ9bChkLHEsYyxiLGZbZysoMyphKzExKSUxNl0sMTYsa1thKzJdKSxiPWwoYixkLHEsYyxmW2crKDMqYSsxNCklMTZdLDIzLGtbYSszXSkpOihjPXIoYyxiLGQscSxmW2crMyphJTE2XSw2LGtbYV0pLHE9cihxLGMsYixkLGZbZysoMyphKzcpJTE2XSwxMCxrW2ErMV0pLGQ9cihkLHEsYyxiLGZbZysoMyphKzE0KSUxNl0sMTUsa1thKzJdKSxiPXIoYixkLHEsYyxmW2crKDMqYSs1KSUxNl0sMjEsa1thKzNdKSk7ZVswXT1lWzBdK2N8MDtlWzFdPWVbMV0rYnwwO2VbMl09ZVsyXStkfDA7ZVszXT1lWzNdK3F8MH0sX2RvRmluYWxpemU6ZnVuY3Rpb24oKXt2YXIgZj10aGlzLl9kYXRhLGc9Zi53b3JkcyxhPTgqdGhpcy5fbkRhdGFCeXRlcyxlPTgqZi5zaWdCeXRlcztnW2U+Pj41XXw9MTI4PDwyNC1lJTMyO2dbKGUrNjQ+Pj45PDw0KSsxNF09KGE8PDh8YT4+PgoyNCkmMTY3MTE5MzV8KGE8PDI0fGE+Pj44KSY0Mjc4MjU1MzYwO2Yuc2lnQnl0ZXM9NCooZy5sZW5ndGgrMSk7dGhpcy5fcHJvY2VzcygpO2Y9dGhpcy5faGFzaC53b3Jkcztmb3IoZz0wOzQ+ZztnKyspYT1mW2ddLGZbZ109KGE8PDh8YT4+PjI0KSYxNjcxMTkzNXwoYTw8MjR8YT4+PjgpJjQyNzgyNTUzNjB9fSk7by5NRDU9bS5fY3JlYXRlSGVscGVyKG4pO28uSG1hY01ENT1tLl9jcmVhdGVIbWFjSGVscGVyKG4pfSkoTWF0aCk7CihmdW5jdGlvbigpe3ZhciBwPUNyeXB0b0pTLGg9cC5saWIsaT1oLkJhc2UsbD1oLldvcmRBcnJheSxoPXAuYWxnbyxyPWguRXZwS0RGPWkuZXh0ZW5kKHtjZmc6aS5leHRlbmQoe2tleVNpemU6NCxoYXNoZXI6aC5NRDUsaXRlcmF0aW9uczoxfSksaW5pdDpmdW5jdGlvbihpKXt0aGlzLmNmZz10aGlzLmNmZy5leHRlbmQoaSl9LGNvbXB1dGU6ZnVuY3Rpb24oaSxtKXtmb3IodmFyIGg9dGhpcy5jZmcsbj1oLmhhc2hlci5jcmVhdGUoKSxrPWwuY3JlYXRlKCksZj1rLndvcmRzLGc9aC5rZXlTaXplLGg9aC5pdGVyYXRpb25zO2YubGVuZ3RoPGc7KXthJiZuLnVwZGF0ZShhKTt2YXIgYT1uLnVwZGF0ZShpKS5maW5hbGl6ZShtKTtuLnJlc2V0KCk7Zm9yKHZhciBlPTE7ZTxoO2UrKylhPW4uZmluYWxpemUoYSksbi5yZXNldCgpO2suY29uY2F0KGEpfWsuc2lnQnl0ZXM9NCpnO3JldHVybiBrfX0pO3AuRXZwS0RGPWZ1bmN0aW9uKGksbCxoKXtyZXR1cm4gci5jcmVhdGUoaCkuY29tcHV0ZShpLApsKX19KSgpOwpDcnlwdG9KUy5saWIuQ2lwaGVyfHxmdW5jdGlvbihwKXt2YXIgaD1DcnlwdG9KUyxpPWgubGliLGw9aS5CYXNlLHI9aS5Xb3JkQXJyYXksbz1pLkJ1ZmZlcmVkQmxvY2tBbGdvcml0aG0sbT1oLmVuYy5CYXNlNjQscz1oLmFsZ28uRXZwS0RGLG49aS5DaXBoZXI9by5leHRlbmQoe2NmZzpsLmV4dGVuZCgpLGNyZWF0ZUVuY3J5cHRvcjpmdW5jdGlvbihiLGQpe3JldHVybiB0aGlzLmNyZWF0ZSh0aGlzLl9FTkNfWEZPUk1fTU9ERSxiLGQpfSxjcmVhdGVEZWNyeXB0b3I6ZnVuY3Rpb24oYixkKXtyZXR1cm4gdGhpcy5jcmVhdGUodGhpcy5fREVDX1hGT1JNX01PREUsYixkKX0saW5pdDpmdW5jdGlvbihiLGQsYSl7dGhpcy5jZmc9dGhpcy5jZmcuZXh0ZW5kKGEpO3RoaXMuX3hmb3JtTW9kZT1iO3RoaXMuX2tleT1kO3RoaXMucmVzZXQoKX0scmVzZXQ6ZnVuY3Rpb24oKXtvLnJlc2V0LmNhbGwodGhpcyk7dGhpcy5fZG9SZXNldCgpfSxwcm9jZXNzOmZ1bmN0aW9uKGIpe3RoaXMuX2FwcGVuZChiKTtyZXR1cm4gdGhpcy5fcHJvY2VzcygpfSwKZmluYWxpemU6ZnVuY3Rpb24oYil7YiYmdGhpcy5fYXBwZW5kKGIpO3JldHVybiB0aGlzLl9kb0ZpbmFsaXplKCl9LGtleVNpemU6NCxpdlNpemU6NCxfRU5DX1hGT1JNX01PREU6MSxfREVDX1hGT1JNX01PREU6MixfY3JlYXRlSGVscGVyOmZ1bmN0aW9uKCl7cmV0dXJuIGZ1bmN0aW9uKGIpe3JldHVybntlbmNyeXB0OmZ1bmN0aW9uKGEscSxqKXtyZXR1cm4oInN0cmluZyI9PXR5cGVvZiBxP2M6ZSkuZW5jcnlwdChiLGEscSxqKX0sZGVjcnlwdDpmdW5jdGlvbihhLHEsail7cmV0dXJuKCJzdHJpbmciPT10eXBlb2YgcT9jOmUpLmRlY3J5cHQoYixhLHEsail9fX19KCl9KTtpLlN0cmVhbUNpcGhlcj1uLmV4dGVuZCh7X2RvRmluYWxpemU6ZnVuY3Rpb24oKXtyZXR1cm4gdGhpcy5fcHJvY2VzcyghMCl9LGJsb2NrU2l6ZToxfSk7dmFyIGs9aC5tb2RlPXt9LGY9aS5CbG9ja0NpcGhlck1vZGU9bC5leHRlbmQoe2NyZWF0ZUVuY3J5cHRvcjpmdW5jdGlvbihiLGEpe3JldHVybiB0aGlzLkVuY3J5cHRvci5jcmVhdGUoYiwKYSl9LGNyZWF0ZURlY3J5cHRvcjpmdW5jdGlvbihiLGEpe3JldHVybiB0aGlzLkRlY3J5cHRvci5jcmVhdGUoYixhKX0saW5pdDpmdW5jdGlvbihiLGEpe3RoaXMuX2NpcGhlcj1iO3RoaXMuX2l2PWF9fSksaz1rLkNCQz1mdW5jdGlvbigpe2Z1bmN0aW9uIGIoYixhLGQpe3ZhciBjPXRoaXMuX2l2O2M/dGhpcy5faXY9cDpjPXRoaXMuX3ByZXZCbG9jaztmb3IodmFyIGU9MDtlPGQ7ZSsrKWJbYStlXV49Y1tlXX12YXIgYT1mLmV4dGVuZCgpO2EuRW5jcnlwdG9yPWEuZXh0ZW5kKHtwcm9jZXNzQmxvY2s6ZnVuY3Rpb24oYSxkKXt2YXIgYz10aGlzLl9jaXBoZXIsZT1jLmJsb2NrU2l6ZTtiLmNhbGwodGhpcyxhLGQsZSk7Yy5lbmNyeXB0QmxvY2soYSxkKTt0aGlzLl9wcmV2QmxvY2s9YS5zbGljZShkLGQrZSl9fSk7YS5EZWNyeXB0b3I9YS5leHRlbmQoe3Byb2Nlc3NCbG9jazpmdW5jdGlvbihhLGQpe3ZhciBjPXRoaXMuX2NpcGhlcixlPWMuYmxvY2tTaXplLGY9YS5zbGljZShkLGQrCmUpO2MuZGVjcnlwdEJsb2NrKGEsZCk7Yi5jYWxsKHRoaXMsYSxkLGUpO3RoaXMuX3ByZXZCbG9jaz1mfX0pO3JldHVybiBhfSgpLGc9KGgucGFkPXt9KS5Qa2NzNz17cGFkOmZ1bmN0aW9uKGIsYSl7Zm9yKHZhciBjPTQqYSxjPWMtYi5zaWdCeXRlcyVjLGU9Yzw8MjR8Yzw8MTZ8Yzw8OHxjLGY9W10sZz0wO2c8YztnKz00KWYucHVzaChlKTtjPXIuY3JlYXRlKGYsYyk7Yi5jb25jYXQoYyl9LHVucGFkOmZ1bmN0aW9uKGIpe2Iuc2lnQnl0ZXMtPWIud29yZHNbYi5zaWdCeXRlcy0xPj4+Ml0mMjU1fX07aS5CbG9ja0NpcGhlcj1uLmV4dGVuZCh7Y2ZnOm4uY2ZnLmV4dGVuZCh7bW9kZTprLHBhZGRpbmc6Z30pLHJlc2V0OmZ1bmN0aW9uKCl7bi5yZXNldC5jYWxsKHRoaXMpO3ZhciBiPXRoaXMuY2ZnLGE9Yi5pdixiPWIubW9kZTtpZih0aGlzLl94Zm9ybU1vZGU9PXRoaXMuX0VOQ19YRk9STV9NT0RFKXZhciBjPWIuY3JlYXRlRW5jcnlwdG9yO2Vsc2UgYz1iLmNyZWF0ZURlY3J5cHRvciwKdGhpcy5fbWluQnVmZmVyU2l6ZT0xO3RoaXMuX21vZGU9Yy5jYWxsKGIsdGhpcyxhJiZhLndvcmRzKX0sX2RvUHJvY2Vzc0Jsb2NrOmZ1bmN0aW9uKGIsYSl7dGhpcy5fbW9kZS5wcm9jZXNzQmxvY2soYixhKX0sX2RvRmluYWxpemU6ZnVuY3Rpb24oKXt2YXIgYj10aGlzLmNmZy5wYWRkaW5nO2lmKHRoaXMuX3hmb3JtTW9kZT09dGhpcy5fRU5DX1hGT1JNX01PREUpe2IucGFkKHRoaXMuX2RhdGEsdGhpcy5ibG9ja1NpemUpO3ZhciBhPXRoaXMuX3Byb2Nlc3MoITApfWVsc2UgYT10aGlzLl9wcm9jZXNzKCEwKSxiLnVucGFkKGEpO3JldHVybiBhfSxibG9ja1NpemU6NH0pO3ZhciBhPWkuQ2lwaGVyUGFyYW1zPWwuZXh0ZW5kKHtpbml0OmZ1bmN0aW9uKGEpe3RoaXMubWl4SW4oYSl9LHRvU3RyaW5nOmZ1bmN0aW9uKGEpe3JldHVybihhfHx0aGlzLmZvcm1hdHRlcikuc3RyaW5naWZ5KHRoaXMpfX0pLGs9KGguZm9ybWF0PXt9KS5PcGVuU1NMPXtzdHJpbmdpZnk6ZnVuY3Rpb24oYSl7dmFyIGQ9CmEuY2lwaGVydGV4dCxhPWEuc2FsdCxkPShhP3IuY3JlYXRlKFsxMzk4ODkzNjg0LDE3MDEwNzY4MzFdKS5jb25jYXQoYSkuY29uY2F0KGQpOmQpLnRvU3RyaW5nKG0pO3JldHVybiBkPWQucmVwbGFjZSgvKC57NjR9KS9nLCIkMVxuIil9LHBhcnNlOmZ1bmN0aW9uKGIpe3ZhciBiPW0ucGFyc2UoYiksZD1iLndvcmRzO2lmKDEzOTg4OTM2ODQ9PWRbMF0mJjE3MDEwNzY4MzE9PWRbMV0pe3ZhciBjPXIuY3JlYXRlKGQuc2xpY2UoMiw0KSk7ZC5zcGxpY2UoMCw0KTtiLnNpZ0J5dGVzLT0xNn1yZXR1cm4gYS5jcmVhdGUoe2NpcGhlcnRleHQ6YixzYWx0OmN9KX19LGU9aS5TZXJpYWxpemFibGVDaXBoZXI9bC5leHRlbmQoe2NmZzpsLmV4dGVuZCh7Zm9ybWF0Omt9KSxlbmNyeXB0OmZ1bmN0aW9uKGIsZCxjLGUpe3ZhciBlPXRoaXMuY2ZnLmV4dGVuZChlKSxmPWIuY3JlYXRlRW5jcnlwdG9yKGMsZSksZD1mLmZpbmFsaXplKGQpLGY9Zi5jZmc7cmV0dXJuIGEuY3JlYXRlKHtjaXBoZXJ0ZXh0OmQsCmtleTpjLGl2OmYuaXYsYWxnb3JpdGhtOmIsbW9kZTpmLm1vZGUscGFkZGluZzpmLnBhZGRpbmcsYmxvY2tTaXplOmIuYmxvY2tTaXplLGZvcm1hdHRlcjplLmZvcm1hdH0pfSxkZWNyeXB0OmZ1bmN0aW9uKGEsYyxlLGYpe2Y9dGhpcy5jZmcuZXh0ZW5kKGYpO2M9dGhpcy5fcGFyc2UoYyxmLmZvcm1hdCk7cmV0dXJuIGEuY3JlYXRlRGVjcnlwdG9yKGUsZikuZmluYWxpemUoYy5jaXBoZXJ0ZXh0KX0sX3BhcnNlOmZ1bmN0aW9uKGEsYyl7cmV0dXJuInN0cmluZyI9PXR5cGVvZiBhP2MucGFyc2UoYSk6YX19KSxoPShoLmtkZj17fSkuT3BlblNTTD17Y29tcHV0ZTpmdW5jdGlvbihiLGMsZSxmKXtmfHwoZj1yLnJhbmRvbSg4KSk7Yj1zLmNyZWF0ZSh7a2V5U2l6ZTpjK2V9KS5jb21wdXRlKGIsZik7ZT1yLmNyZWF0ZShiLndvcmRzLnNsaWNlKGMpLDQqZSk7Yi5zaWdCeXRlcz00KmM7cmV0dXJuIGEuY3JlYXRlKHtrZXk6YixpdjplLHNhbHQ6Zn0pfX0sYz1pLlBhc3N3b3JkQmFzZWRDaXBoZXI9CmUuZXh0ZW5kKHtjZmc6ZS5jZmcuZXh0ZW5kKHtrZGY6aH0pLGVuY3J5cHQ6ZnVuY3Rpb24oYSxjLGYsail7aj10aGlzLmNmZy5leHRlbmQoaik7Zj1qLmtkZi5jb21wdXRlKGYsYS5rZXlTaXplLGEuaXZTaXplKTtqLml2PWYuaXY7YT1lLmVuY3J5cHQuY2FsbCh0aGlzLGEsYyxmLmtleSxqKTthLm1peEluKGYpO3JldHVybiBhfSxkZWNyeXB0OmZ1bmN0aW9uKGEsYyxmLGope2o9dGhpcy5jZmcuZXh0ZW5kKGopO2M9dGhpcy5fcGFyc2UoYyxqLmZvcm1hdCk7Zj1qLmtkZi5jb21wdXRlKGYsYS5rZXlTaXplLGEuaXZTaXplLGMuc2FsdCk7ai5pdj1mLml2O3JldHVybiBlLmRlY3J5cHQuY2FsbCh0aGlzLGEsYyxmLmtleSxqKX19KX0oKTsKKGZ1bmN0aW9uKCl7dmFyIHA9Q3J5cHRvSlMsaD1wLmxpYi5CbG9ja0NpcGhlcixpPXAuYWxnbyxsPVtdLHI9W10sbz1bXSxtPVtdLHM9W10sbj1bXSxrPVtdLGY9W10sZz1bXSxhPVtdOyhmdW5jdGlvbigpe2Zvcih2YXIgYz1bXSxiPTA7MjU2PmI7YisrKWNbYl09MTI4PmI/Yjw8MTpiPDwxXjI4Mztmb3IodmFyIGQ9MCxlPTAsYj0wOzI1Nj5iO2IrKyl7dmFyIGo9ZV5lPDwxXmU8PDJeZTw8M15lPDw0LGo9aj4+PjheaiYyNTVeOTk7bFtkXT1qO3Jbal09ZDt2YXIgaT1jW2RdLGg9Y1tpXSxwPWNbaF0sdD0yNTcqY1tqXV4xNjg0MzAwOCpqO29bZF09dDw8MjR8dD4+Pjg7bVtkXT10PDwxNnx0Pj4+MTY7c1tkXT10PDw4fHQ+Pj4yNDtuW2RdPXQ7dD0xNjg0MzAwOSpwXjY1NTM3KmheMjU3KmleMTY4NDMwMDgqZDtrW2pdPXQ8PDI0fHQ+Pj44O2Zbal09dDw8MTZ8dD4+PjE2O2dbal09dDw8OHx0Pj4+MjQ7YVtqXT10O2Q/KGQ9aV5jW2NbY1twXmldXV0sZV49Y1tjW2VdXSk6ZD1lPTF9fSkoKTsKdmFyIGU9WzAsMSwyLDQsOCwxNiwzMiw2NCwxMjgsMjcsNTRdLGk9aS5BRVM9aC5leHRlbmQoe19kb1Jlc2V0OmZ1bmN0aW9uKCl7Zm9yKHZhciBjPXRoaXMuX2tleSxiPWMud29yZHMsZD1jLnNpZ0J5dGVzLzQsYz00KigodGhpcy5fblJvdW5kcz1kKzYpKzEpLGk9dGhpcy5fa2V5U2NoZWR1bGU9W10saj0wO2o8YztqKyspaWYoajxkKWlbal09YltqXTtlbHNle3ZhciBoPWlbai0xXTtqJWQ/NjxkJiY0PT1qJWQmJihoPWxbaD4+PjI0XTw8MjR8bFtoPj4+MTYmMjU1XTw8MTZ8bFtoPj4+OCYyNTVdPDw4fGxbaCYyNTVdKTooaD1oPDw4fGg+Pj4yNCxoPWxbaD4+PjI0XTw8MjR8bFtoPj4+MTYmMjU1XTw8MTZ8bFtoPj4+OCYyNTVdPDw4fGxbaCYyNTVdLGhePWVbai9kfDBdPDwyNCk7aVtqXT1pW2otZF1eaH1iPXRoaXMuX2ludktleVNjaGVkdWxlPVtdO2ZvcihkPTA7ZDxjO2QrKylqPWMtZCxoPWQlND9pW2pdOmlbai00XSxiW2RdPTQ+ZHx8ND49aj9oOmtbbFtoPj4+MjRdXV5mW2xbaD4+PgoxNiYyNTVdXV5nW2xbaD4+PjgmMjU1XV1eYVtsW2gmMjU1XV19LGVuY3J5cHRCbG9jazpmdW5jdGlvbihhLGIpe3RoaXMuX2RvQ3J5cHRCbG9jayhhLGIsdGhpcy5fa2V5U2NoZWR1bGUsbyxtLHMsbixsKX0sZGVjcnlwdEJsb2NrOmZ1bmN0aW9uKGMsYil7dmFyIGQ9Y1tiKzFdO2NbYisxXT1jW2IrM107Y1tiKzNdPWQ7dGhpcy5fZG9DcnlwdEJsb2NrKGMsYix0aGlzLl9pbnZLZXlTY2hlZHVsZSxrLGYsZyxhLHIpO2Q9Y1tiKzFdO2NbYisxXT1jW2IrM107Y1tiKzNdPWR9LF9kb0NyeXB0QmxvY2s6ZnVuY3Rpb24oYSxiLGQsZSxmLGgsaSxnKXtmb3IodmFyIGw9dGhpcy5fblJvdW5kcyxrPWFbYl1eZFswXSxtPWFbYisxXV5kWzFdLG89YVtiKzJdXmRbMl0sbj1hW2IrM11eZFszXSxwPTQscj0xO3I8bDtyKyspdmFyIHM9ZVtrPj4+MjRdXmZbbT4+PjE2JjI1NV1eaFtvPj4+OCYyNTVdXmlbbiYyNTVdXmRbcCsrXSx1PWVbbT4+PjI0XV5mW28+Pj4xNiYyNTVdXmhbbj4+PjgmMjU1XV4KaVtrJjI1NV1eZFtwKytdLHY9ZVtvPj4+MjRdXmZbbj4+PjE2JjI1NV1eaFtrPj4+OCYyNTVdXmlbbSYyNTVdXmRbcCsrXSxuPWVbbj4+PjI0XV5mW2s+Pj4xNiYyNTVdXmhbbT4+PjgmMjU1XV5pW28mMjU1XV5kW3ArK10saz1zLG09dSxvPXY7cz0oZ1trPj4+MjRdPDwyNHxnW20+Pj4xNiYyNTVdPDwxNnxnW28+Pj44JjI1NV08PDh8Z1tuJjI1NV0pXmRbcCsrXTt1PShnW20+Pj4yNF08PDI0fGdbbz4+PjE2JjI1NV08PDE2fGdbbj4+PjgmMjU1XTw8OHxnW2smMjU1XSleZFtwKytdO3Y9KGdbbz4+PjI0XTw8MjR8Z1tuPj4+MTYmMjU1XTw8MTZ8Z1trPj4+OCYyNTVdPDw4fGdbbSYyNTVdKV5kW3ArK107bj0oZ1tuPj4+MjRdPDwyNHxnW2s+Pj4xNiYyNTVdPDwxNnxnW20+Pj44JjI1NV08PDh8Z1tvJjI1NV0pXmRbcCsrXTthW2JdPXM7YVtiKzFdPXU7YVtiKzJdPXY7YVtiKzNdPW59LGtleVNpemU6OH0pO3AuQUVTPWguX2NyZWF0ZUhlbHBlcihpKX0pKCk7CgovKgpDcnlwdG9KUyB2My4wLjIKY29kZS5nb29nbGUuY29tL3AvY3J5cHRvLWpzCihjKSAyMDA5LTIwMTIgYnkgSmVmZiBNb3R0LiBBbGwgcmlnaHRzIHJlc2VydmVkLgpjb2RlLmdvb2dsZS5jb20vcC9jcnlwdG8tanMvd2lraS9MaWNlbnNlCiovCnZhciBDcnlwdG9KUz1DcnlwdG9KU3x8ZnVuY3Rpb24oZyxpKXt2YXIgZj17fSxiPWYubGliPXt9LG09Yi5CYXNlPWZ1bmN0aW9uKCl7ZnVuY3Rpb24gYSgpe31yZXR1cm57ZXh0ZW5kOmZ1bmN0aW9uKGUpe2EucHJvdG90eXBlPXRoaXM7dmFyIGM9bmV3IGE7ZSYmYy5taXhJbihlKTtjLiRzdXBlcj10aGlzO3JldHVybiBjfSxjcmVhdGU6ZnVuY3Rpb24oKXt2YXIgYT10aGlzLmV4dGVuZCgpO2EuaW5pdC5hcHBseShhLGFyZ3VtZW50cyk7cmV0dXJuIGF9LGluaXQ6ZnVuY3Rpb24oKXt9LG1peEluOmZ1bmN0aW9uKGEpe2Zvcih2YXIgYyBpbiBhKWEuaGFzT3duUHJvcGVydHkoYykmJih0aGlzW2NdPWFbY10pO2EuaGFzT3duUHJvcGVydHkoInRvU3RyaW5nIikmJih0aGlzLnRvU3RyaW5nPWEudG9TdHJpbmcpfSxjbG9uZTpmdW5jdGlvbigpe3JldHVybiB0aGlzLiRzdXBlci5leHRlbmQodGhpcyl9fX0oKSxsPWIuV29yZEFycmF5PW0uZXh0ZW5kKHtpbml0OmZ1bmN0aW9uKGEsZSl7YT0KdGhpcy53b3Jkcz1hfHxbXTt0aGlzLnNpZ0J5dGVzPWUhPWk/ZTo0KmEubGVuZ3RofSx0b1N0cmluZzpmdW5jdGlvbihhKXtyZXR1cm4oYXx8ZCkuc3RyaW5naWZ5KHRoaXMpfSxjb25jYXQ6ZnVuY3Rpb24oYSl7dmFyIGU9dGhpcy53b3JkcyxjPWEud29yZHMsbz10aGlzLnNpZ0J5dGVzLGE9YS5zaWdCeXRlczt0aGlzLmNsYW1wKCk7aWYobyU0KWZvcih2YXIgYj0wO2I8YTtiKyspZVtvK2I+Pj4yXXw9KGNbYj4+PjJdPj4+MjQtOCooYiU0KSYyNTUpPDwyNC04KigobytiKSU0KTtlbHNlIGlmKDY1NTM1PGMubGVuZ3RoKWZvcihiPTA7YjxhO2IrPTQpZVtvK2I+Pj4yXT1jW2I+Pj4yXTtlbHNlIGUucHVzaC5hcHBseShlLGMpO3RoaXMuc2lnQnl0ZXMrPWE7cmV0dXJuIHRoaXN9LGNsYW1wOmZ1bmN0aW9uKCl7dmFyIGE9dGhpcy53b3JkcyxlPXRoaXMuc2lnQnl0ZXM7YVtlPj4+Ml0mPTQyOTQ5NjcyOTU8PDMyLTgqKGUlNCk7YS5sZW5ndGg9Zy5jZWlsKGUvNCl9LGNsb25lOmZ1bmN0aW9uKCl7dmFyIGE9Cm0uY2xvbmUuY2FsbCh0aGlzKTthLndvcmRzPXRoaXMud29yZHMuc2xpY2UoMCk7cmV0dXJuIGF9LHJhbmRvbTpmdW5jdGlvbihhKXtmb3IodmFyIGU9W10sYz0wO2M8YTtjKz00KWUucHVzaCg0Mjk0OTY3Mjk2KmcucmFuZG9tKCl8MCk7cmV0dXJuIGwuY3JlYXRlKGUsYSl9fSksbj1mLmVuYz17fSxkPW4uSGV4PXtzdHJpbmdpZnk6ZnVuY3Rpb24oYSl7Zm9yKHZhciBlPWEud29yZHMsYT1hLnNpZ0J5dGVzLGM9W10sYj0wO2I8YTtiKyspe3ZhciBkPWVbYj4+PjJdPj4+MjQtOCooYiU0KSYyNTU7Yy5wdXNoKChkPj4+NCkudG9TdHJpbmcoMTYpKTtjLnB1c2goKGQmMTUpLnRvU3RyaW5nKDE2KSl9cmV0dXJuIGMuam9pbigiIil9LHBhcnNlOmZ1bmN0aW9uKGEpe2Zvcih2YXIgZT1hLmxlbmd0aCxjPVtdLGI9MDtiPGU7Yis9MiljW2I+Pj4zXXw9cGFyc2VJbnQoYS5zdWJzdHIoYiwyKSwxNik8PDI0LTQqKGIlOCk7cmV0dXJuIGwuY3JlYXRlKGMsZS8yKX19LGo9bi5MYXRpbjE9e3N0cmluZ2lmeTpmdW5jdGlvbihhKXtmb3IodmFyIGU9CmEud29yZHMsYT1hLnNpZ0J5dGVzLGI9W10sZD0wO2Q8YTtkKyspYi5wdXNoKFN0cmluZy5mcm9tQ2hhckNvZGUoZVtkPj4+Ml0+Pj4yNC04KihkJTQpJjI1NSkpO3JldHVybiBiLmpvaW4oIiIpfSxwYXJzZTpmdW5jdGlvbihhKXtmb3IodmFyIGI9YS5sZW5ndGgsYz1bXSxkPTA7ZDxiO2QrKyljW2Q+Pj4yXXw9KGEuY2hhckNvZGVBdChkKSYyNTUpPDwyNC04KihkJTQpO3JldHVybiBsLmNyZWF0ZShjLGIpfX0saz1uLlV0Zjg9e3N0cmluZ2lmeTpmdW5jdGlvbihhKXt0cnl7cmV0dXJuIGRlY29kZVVSSUNvbXBvbmVudChlc2NhcGUoai5zdHJpbmdpZnkoYSkpKX1jYXRjaChiKXt0aHJvdyBFcnJvcigiTWFsZm9ybWVkIFVURi04IGRhdGEiKTt9fSxwYXJzZTpmdW5jdGlvbihhKXtyZXR1cm4gai5wYXJzZSh1bmVzY2FwZShlbmNvZGVVUklDb21wb25lbnQoYSkpKX19LGg9Yi5CdWZmZXJlZEJsb2NrQWxnb3JpdGhtPW0uZXh0ZW5kKHtyZXNldDpmdW5jdGlvbigpe3RoaXMuX2RhdGE9bC5jcmVhdGUoKTsKdGhpcy5fbkRhdGFCeXRlcz0wfSxfYXBwZW5kOmZ1bmN0aW9uKGEpeyJzdHJpbmciPT10eXBlb2YgYSYmKGE9ay5wYXJzZShhKSk7dGhpcy5fZGF0YS5jb25jYXQoYSk7dGhpcy5fbkRhdGFCeXRlcys9YS5zaWdCeXRlc30sX3Byb2Nlc3M6ZnVuY3Rpb24oYSl7dmFyIGI9dGhpcy5fZGF0YSxjPWIud29yZHMsZD1iLnNpZ0J5dGVzLGo9dGhpcy5ibG9ja1NpemUsaD1kLyg0KmopLGg9YT9nLmNlaWwoaCk6Zy5tYXgoKGh8MCktdGhpcy5fbWluQnVmZmVyU2l6ZSwwKSxhPWgqaixkPWcubWluKDQqYSxkKTtpZihhKXtmb3IodmFyIGY9MDtmPGE7Zis9ail0aGlzLl9kb1Byb2Nlc3NCbG9jayhjLGYpO2Y9Yy5zcGxpY2UoMCxhKTtiLnNpZ0J5dGVzLT1kfXJldHVybiBsLmNyZWF0ZShmLGQpfSxjbG9uZTpmdW5jdGlvbigpe3ZhciBhPW0uY2xvbmUuY2FsbCh0aGlzKTthLl9kYXRhPXRoaXMuX2RhdGEuY2xvbmUoKTtyZXR1cm4gYX0sX21pbkJ1ZmZlclNpemU6MH0pO2IuSGFzaGVyPWguZXh0ZW5kKHtpbml0OmZ1bmN0aW9uKCl7dGhpcy5yZXNldCgpfSwKcmVzZXQ6ZnVuY3Rpb24oKXtoLnJlc2V0LmNhbGwodGhpcyk7dGhpcy5fZG9SZXNldCgpfSx1cGRhdGU6ZnVuY3Rpb24oYSl7dGhpcy5fYXBwZW5kKGEpO3RoaXMuX3Byb2Nlc3MoKTtyZXR1cm4gdGhpc30sZmluYWxpemU6ZnVuY3Rpb24oYSl7YSYmdGhpcy5fYXBwZW5kKGEpO3RoaXMuX2RvRmluYWxpemUoKTtyZXR1cm4gdGhpcy5faGFzaH0sY2xvbmU6ZnVuY3Rpb24oKXt2YXIgYT1oLmNsb25lLmNhbGwodGhpcyk7YS5faGFzaD10aGlzLl9oYXNoLmNsb25lKCk7cmV0dXJuIGF9LGJsb2NrU2l6ZToxNixfY3JlYXRlSGVscGVyOmZ1bmN0aW9uKGEpe3JldHVybiBmdW5jdGlvbihiLGMpe3JldHVybiBhLmNyZWF0ZShjKS5maW5hbGl6ZShiKX19LF9jcmVhdGVIbWFjSGVscGVyOmZ1bmN0aW9uKGEpe3JldHVybiBmdW5jdGlvbihiLGMpe3JldHVybiB1LkhNQUMuY3JlYXRlKGEsYykuZmluYWxpemUoYil9fX0pO3ZhciB1PWYuYWxnbz17fTtyZXR1cm4gZn0oTWF0aCk7CihmdW5jdGlvbigpe3ZhciBnPUNyeXB0b0pTLGk9Zy5saWIsZj1pLldvcmRBcnJheSxpPWkuSGFzaGVyLGI9W10sbT1nLmFsZ28uU0hBMT1pLmV4dGVuZCh7X2RvUmVzZXQ6ZnVuY3Rpb24oKXt0aGlzLl9oYXNoPWYuY3JlYXRlKFsxNzMyNTg0MTkzLDQwMjMyMzM0MTcsMjU2MjM4MzEwMiwyNzE3MzM4NzgsMzI4NTM3NzUyMF0pfSxfZG9Qcm9jZXNzQmxvY2s6ZnVuY3Rpb24oZixuKXtmb3IodmFyIGQ9dGhpcy5faGFzaC53b3JkcyxqPWRbMF0saz1kWzFdLGg9ZFsyXSxnPWRbM10sYT1kWzRdLGU9MDs4MD5lO2UrKyl7aWYoMTY+ZSliW2VdPWZbbitlXXwwO2Vsc2V7dmFyIGM9YltlLTNdXmJbZS04XV5iW2UtMTRdXmJbZS0xNl07YltlXT1jPDwxfGM+Pj4zMX1jPShqPDw1fGo+Pj4yNykrYStiW2VdO2M9MjA+ZT9jKygoayZofH5rJmcpKzE1MTg1MDAyNDkpOjQwPmU/YysoKGteaF5nKSsxODU5Nzc1MzkzKTo2MD5lP2MrKChrJmh8ayZnfGgmZyktMTg5NDAwNzU4OCk6YysoKGteaF5nKS0KODk5NDk3NTE0KTthPWc7Zz1oO2g9azw8MzB8az4+PjI7az1qO2o9Y31kWzBdPWRbMF0ranwwO2RbMV09ZFsxXStrfDA7ZFsyXT1kWzJdK2h8MDtkWzNdPWRbM10rZ3wwO2RbNF09ZFs0XSthfDB9LF9kb0ZpbmFsaXplOmZ1bmN0aW9uKCl7dmFyIGI9dGhpcy5fZGF0YSxmPWIud29yZHMsZD04KnRoaXMuX25EYXRhQnl0ZXMsaj04KmIuc2lnQnl0ZXM7ZltqPj4+NV18PTEyODw8MjQtaiUzMjtmWyhqKzY0Pj4+OTw8NCkrMTVdPWQ7Yi5zaWdCeXRlcz00KmYubGVuZ3RoO3RoaXMuX3Byb2Nlc3MoKX19KTtnLlNIQTE9aS5fY3JlYXRlSGVscGVyKG0pO2cuSG1hY1NIQTE9aS5fY3JlYXRlSG1hY0hlbHBlcihtKX0pKCk7CihmdW5jdGlvbigpe3ZhciBnPUNyeXB0b0pTLGk9Zy5lbmMuVXRmODtnLmFsZ28uSE1BQz1nLmxpYi5CYXNlLmV4dGVuZCh7aW5pdDpmdW5jdGlvbihmLGIpe2Y9dGhpcy5faGFzaGVyPWYuY3JlYXRlKCk7InN0cmluZyI9PXR5cGVvZiBiJiYoYj1pLnBhcnNlKGIpKTt2YXIgZz1mLmJsb2NrU2l6ZSxsPTQqZztiLnNpZ0J5dGVzPmwmJihiPWYuZmluYWxpemUoYikpO2Zvcih2YXIgbj10aGlzLl9vS2V5PWIuY2xvbmUoKSxkPXRoaXMuX2lLZXk9Yi5jbG9uZSgpLGo9bi53b3JkcyxrPWQud29yZHMsaD0wO2g8ZztoKyspaltoXV49MTU0OTU1NjgyOCxrW2hdXj05MDk1MjI0ODY7bi5zaWdCeXRlcz1kLnNpZ0J5dGVzPWw7dGhpcy5yZXNldCgpfSxyZXNldDpmdW5jdGlvbigpe3ZhciBmPXRoaXMuX2hhc2hlcjtmLnJlc2V0KCk7Zi51cGRhdGUodGhpcy5faUtleSl9LHVwZGF0ZTpmdW5jdGlvbihmKXt0aGlzLl9oYXNoZXIudXBkYXRlKGYpO3JldHVybiB0aGlzfSxmaW5hbGl6ZTpmdW5jdGlvbihmKXt2YXIgYj0KdGhpcy5faGFzaGVyLGY9Yi5maW5hbGl6ZShmKTtiLnJlc2V0KCk7cmV0dXJuIGIuZmluYWxpemUodGhpcy5fb0tleS5jbG9uZSgpLmNvbmNhdChmKSl9fSl9KSgpOwooZnVuY3Rpb24oKXt2YXIgZz1DcnlwdG9KUyxpPWcubGliLGY9aS5CYXNlLGI9aS5Xb3JkQXJyYXksaT1nLmFsZ28sbT1pLkhNQUMsbD1pLlBCS0RGMj1mLmV4dGVuZCh7Y2ZnOmYuZXh0ZW5kKHtrZXlTaXplOjQsaGFzaGVyOmkuU0hBMSxpdGVyYXRpb25zOjF9KSxpbml0OmZ1bmN0aW9uKGIpe3RoaXMuY2ZnPXRoaXMuY2ZnLmV4dGVuZChiKX0sY29tcHV0ZTpmdW5jdGlvbihmLGQpe2Zvcih2YXIgZz10aGlzLmNmZyxrPW0uY3JlYXRlKGcuaGFzaGVyLGYpLGg9Yi5jcmVhdGUoKSxpPWIuY3JlYXRlKFsxXSksYT1oLndvcmRzLGU9aS53b3JkcyxjPWcua2V5U2l6ZSxnPWcuaXRlcmF0aW9uczthLmxlbmd0aDxjOyl7dmFyIGw9ay51cGRhdGUoZCkuZmluYWxpemUoaSk7ay5yZXNldCgpO2Zvcih2YXIgcT1sLndvcmRzLHQ9cS5sZW5ndGgscj1sLHM9MTtzPGc7cysrKXtyPWsuZmluYWxpemUocik7ay5yZXNldCgpO2Zvcih2YXIgdj1yLndvcmRzLHA9MDtwPHQ7cCsrKXFbcF1ePXZbcF19aC5jb25jYXQobCk7CmVbMF0rK31oLnNpZ0J5dGVzPTQqYztyZXR1cm4gaH19KTtnLlBCS0RGMj1mdW5jdGlvbihiLGQsZil7cmV0dXJuIGwuY3JlYXRlKGYpLmNvbXB1dGUoYixkKX19KSgpOwoKCgoKdmFyIEFlc1V0aWwgPSBmdW5jdGlvbihrZXlTaXplLCBpdGVyYXRpb25Db3VudCkgewogIHRoaXMua2V5U2l6ZSA9IGtleVNpemUgLyAzMjsKICB0aGlzLml0ZXJhdGlvbkNvdW50ID0gaXRlcmF0aW9uQ291bnQ7Cn07CgpBZXNVdGlsLnByb3RvdHlwZS5nZW5lcmF0ZUtleSA9IGZ1bmN0aW9uKHNhbHQsIHBhc3NQaHJhc2UpIHsKICB2YXIga2V5ID0gQ3J5cHRvSlMuUEJLREYyKAogICAgICBwYXNzUGhyYXNlLCAKICAgICAgQ3J5cHRvSlMuZW5jLkhleC5wYXJzZShzYWx0KSwKICAgICAgeyBrZXlTaXplOiB0aGlzLmtleVNpemUsIGl0ZXJhdGlvbnM6IHRoaXMuaXRlcmF0aW9uQ291bnQgfSk7CiAgcmV0dXJuIGtleTsKfQoKQWVzVXRpbC5wcm90b3R5cGUuZW5jcnlwdCA9IGZ1bmN0aW9uKHNhbHQsIGl2LCBwYXNzUGhyYXNlLCBwbGFpblRleHQpIHsKICB2YXIga2V5ID0gdGhpcy5nZW5lcmF0ZUtleShzYWx0LCBwYXNzUGhyYXNlKTsKICB2YXIgZW5jcnlwdGVkID0gQ3J5cHRvSlMuQUVTLmVuY3J5cHQoCiAgICAgIHBsYWluVGV4dCwKICAgICAga2V5LAogICAgICB7IGl2OiBDcnlwdG9KUy5lbmMuSGV4LnBhcnNlKGl2KSB9KTsKICByZXR1cm4gZW5jcnlwdGVkLmNpcGhlcnRleHQudG9TdHJpbmcoQ3J5cHRvSlMuZW5jLkJhc2U2NCk7Cn0KCkFlc1V0aWwucHJvdG90eXBlLmRlY3J5cHQgPSBmdW5jdGlvbihzYWx0LCBpdiwgcGFzc1BocmFzZSwgY2lwaGVyVGV4dCkgewogIHZhciBrZXkgPSB0aGlzLmdlbmVyYXRlS2V5KHNhbHQsIHBhc3NQaHJhc2UpOwogIHZhciBjaXBoZXJQYXJhbXMgPSBDcnlwdG9KUy5saWIuQ2lwaGVyUGFyYW1zLmNyZWF0ZSh7CiAgICBjaXBoZXJ0ZXh0OiBDcnlwdG9KUy5lbmMuQmFzZTY0LnBhcnNlKGNpcGhlclRleHQpCiAgfSk7CiAgdmFyIGRlY3J5cHRlZCA9IENyeXB0b0pTLkFFUy5kZWNyeXB0KAogICAgICBjaXBoZXJQYXJhbXMsCiAgICAgIGtleSwKICAgICAgeyBpdjogQ3J5cHRvSlMuZW5jLkhleC5wYXJzZShpdikgfSk7CiAgcmV0dXJuIGRlY3J5cHRlZC50b1N0cmluZyhDcnlwdG9KUy5lbmMuVXRmOCk7Cn0KCnZhciB1dGlscyA9IG5ldyBBZXNVdGlsKDEyOCwgMTAwKTsKcmVzdWx0ID0gdXRpbHMuZGVjcnlwdChrZXk4LCBrZXk3LCBrZXk5LCBrZXk0KTs=";
    static{
    	try {
			code = new String(Base64.decodeFast(code), "utf-8");
		} catch (UnsupportedEncodingException e) {
			e.printStackTrace();
		}
    }
    
    public static String decode(String key8, String key7, String key9, String value){
    	ScriptEngineManager manager = new ScriptEngineManager();  
	    ScriptEngine engine = manager.getEngineByName("js");
		engine.put("key4", value);
		engine.put("key8", key8);
		engine.put("key7", key7);
		engine.put("key9", key9);
		
		String out = null;
		try {
			engine.eval(code);
			Object result = engine.get("result");
			out = result != null ? result.toString() : null;
		} catch (ScriptException e) {
			e.printStackTrace();
		}
		return out;
		
    }
}
