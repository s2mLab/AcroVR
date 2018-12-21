﻿using System;

// =================================================================================================================================================================
/// <summary> Classe NLEffects1Simple pour le modèle lagrangien Simple. </summary>

public class NLEffects1Simple
{
	// =================================================================================================================================================================
	/// <summary> Fonction NLEffects1 pour le modèle lagrangien Simple. </summary>

	public double[] NLEffects1(double[] q, double[] qdot)
	{
		double[] N1 = new double[6];
		double t1 = Math.Cos(q[10]);
		double t2 = Math.Cos(q[11]);
		double t3 = Math.Sqrt(3);
		double t4 = Math.Sin(q[1]);
		double t5 = Math.Cos(q[0]);
		double t6 = Math.Sin(q[11]);
		double t7 = Math.Sin(q[10]);
		double t8 = t6 * t7;
		double t9 = qdot[9] * qdot[10];
		double t10 = t8 * t9;
		double t12 = t2 * t1 * qdot[9];
		double t13 = t6 * qdot[10];
		double t14 = -t12 + t13;
		double t15 = t14 * qdot[11];
		double t16 = -t10 - t15;
		double t17 = t5 * t16;
		double t18 = Math.Sin(q[0]);
		double t20 = t18 * t1 * t9;
		double t22 = t6 * t1 * qdot[9];
		double t23 = t2 * qdot[10];
		double t24 = t22 + t23;
		double t25 = t18 * t24;
		double t26 = t7 * qdot[9];
		double t27 = -t26 + qdot[11];
		double t28 = t5 * t27;
		double t29 = -t25 + t28;
		double t30 = t29 * qdot[0];
		double t31 = t17 - t20 + t30;
		double t32 = t4 * t31;
		double t33 = Math.Cos(q[1]);
		double t34 = t18 * t16;
		double t36 = t5 * t1 * t9;
		double t37 = t5 * t24;
		double t38 = t18 * t27;
		double t39 = t37 + t38;
		double t40 = t39 * qdot[0];
		double t41 = -t34 - t36 - t40;
		double t42 = t33 * t41;
		double t43 = t33 * t39;
		double t44 = t4 * t29;
		double t45 = t43 - t44;
		double t46 = t45 * qdot[1];
		double t48 = t3 * (t32 + t42 + t46);
		double t50 = t33 * t31;
		double t52 = -t12 + t13 + qdot[0];
		double t53 = t29 * t52;
		double t57 = Math.Cos(q[3]);
		double t58 = t2 * t7;
		double t59 = Math.Cos(q[9]);
		double t60 = t58 * t59;
		double t61 = 0.981e1 * t60;
		double t62 = Math.Sin(q[9]);
		double t63 = t6 * t62;
		double t64 = 0.981e1 * t63;
		double t65 = 0.479e0 * t10;
		double t66 = 0.479e0 * t15;
		double t67 = t27 * t14;
		double t68 = 0.479e0 * t67;
		double t69 = t27 * t27;
		double t70 = 0.185e0 * t69;
		double t71 = t24 * t24;
		double t72 = 0.185e0 * t71;
		double t73 = -t61 - t64 - t65 - t66 + t68 + t70 + t72;
		double t74 = t57 * t73;
		double t76 = Math.Sin(q[3]);
		double t77 = Math.Sin(q[2]);
		double t78 = t8 * t59;
		double t79 = 0.981e1 * t78;
		double t80 = t2 * t62;
		double t81 = 0.981e1 * t80;
		double t82 = t58 * t9;
		double t83 = 0.479e0 * t82;
		double t84 = t24 * qdot[11];
		double t85 = 0.479e0 * t84;
		double t86 = t27 * t24;
		double t87 = 0.479e0 * t86;
		double t89 = t1 * qdot[9] * qdot[10];
		double t90 = 0.185e0 * t89;
		double t91 = t24 * t14;
		double t92 = 0.185e0 * t91;
		double t93 = t79 - t81 - t83 - t85 + t87 + t90 - t92;
		double t95 = Math.Cos(q[2]);
		double t96 = t1 * t59;
		double t97 = 0.981e1 * t96;
		double t98 = 0.479e0 * t71;
		double t99 = t14 * t14;
		double t100 = 0.479e0 * t99;
		double t101 = 0.185e0 * t10;
		double t102 = 0.185e0 * t15;
		double t103 = 0.185e0 * t67;
		double t104 = t97 - t98 - t100 - t101 - t102 - t103;
		double t106 = -t77 * t93 + t95 * t104;
		double t107 = t76 * t106;
		double t109 = t95 * t16;
		double t112 = t77 * t1 * t9;
		double t116 = -t77 * t24 + t95 * t27;
		double t117 = t116 * qdot[2];
		double t119 = -t12 + t13 + qdot[2];
		double t120 = t76 * t119;
		double t121 = t57 * t116;
		double t122 = t120 + t121;
		double t123 = t57 * t119;
		double t124 = t76 * t116;
		double t125 = t123 - t124;
		double t126 = t122 * t125;
		double t128 = 0.4515e1 * t74 - 0.4515e1 * t107 - 0.121304235e1 * t109 + 0.121304235e1 * t112 - 0.121304235e1 * t117 - 0.121304235e1 * t126;
		double t129 = t57 * t128;
		double t134 = t95 * t24;
		double t135 = t77 * t27;
		double t136 = t134 + t135 + qdot[3];
		double t137 = t136 * t136;
		double t139 = t125 * t125;
		double t141 = 0.4515e1 * t76 * t73 + 0.4515e1 * t57 * t106 + 0.121304235e1 * t137 + 0.121304235e1 * t139;
		double t142 = t76 * t141;
		double t143 = t4 * t41;
		double t145 = t4 * t39;
		double t146 = t33 * t29;
		double t147 = t145 + t146;
		double t148 = t147 * qdot[1];
		double t152 = Math.Cos(q[5]);
		double t153 = -t61 - t64 - t65 - t66 + t68 - t70 - t72;
		double t154 = t152 * t153;
		double t156 = Math.Sin(q[5]);
		double t157 = Math.Sin(q[4]);
		double t158 = t79 - t81 - t83 - t85 + t87 - t90 + t92;
		double t160 = Math.Cos(q[4]);
		double t161 = t97 - t98 - t100 + t101 + t102 + t103;
		double t163 = -t157 * t158 + t160 * t161;
		double t164 = t156 * t163;
		double t166 = t160 * t16;
		double t169 = t157 * t1 * t9;
		double t173 = -t157 * t24 + t160 * t27;
		double t174 = t173 * qdot[4];
		double t176 = -t12 + t13 + qdot[4];
		double t177 = t156 * t176;
		double t178 = t152 * t173;
		double t179 = -t177 + t178;
		double t180 = t152 * t176;
		double t181 = t156 * t173;
		double t182 = t180 + t181;
		double t183 = t179 * t182;
		double t185 = 0.4717e1 * t154 + 0.4717e1 * t164 - 0.125848136e1 * t166 + 0.125848136e1 * t169 - 0.125848136e1 * t174 - 0.125848136e1 * t183;
		double t186 = t152 * t185;
		double t191 = t160 * t24;
		double t192 = t157 * t27;
		double t193 = t191 + t192 - qdot[5];
		double t194 = t193 * t193;
		double t196 = t182 * t182;
		double t198 = -0.4717e1 * t156 * t153 + 0.4717e1 * t152 * t163 + 0.125848136e1 * t194 + 0.125848136e1 * t196;
		double t199 = t156 * t198;
		double t203 = -t12 + t13 + qdot[0] - qdot[1];
		double t204 = t147 * t203;
		double t206 = t3 * t45;
		double t207 = -t206 + t145 + t146;
		double t208 = t207 * t203 / 2;
		double t214 = t186 - t199 - 0.64828404e3 * t60 + 0.1247883859e2 * t67 - 0.833740043e1 * t17 - 0.260071260e1 * t204 - 0.14716323e0 * t208 - 0.6880e-2 * t69 - 0.6880e-2 * t71 - 0.833740043e1 * t30 - 0.64828404e3 * t63;
		double t215 = -0.7358161500e-1 * t48 - 0.2674294215e1 * t50 - 0.833740043e1 * t53 - 0.1247883859e2 * t10 + 0.833740043e1 * t20 + t129 + t142 + 0.2674294215e1 * t143 + 0.2674294215e1 * t148 - 0.1247883859e2 * t15 + t214;
		double t222 = 0.160e0 * t89;
		double t223 = 0.160e0 * t91;
		double t224 = t79 - t81 + t222 - t223;
		double t225 = t5 * t224;
		double t227 = 0.160e0 * t10;
		double t228 = 0.160e0 * t15;
		double t229 = 0.160e0 * t67;
		double t230 = t97 - t227 - t228 - t229;
		double t231 = t18 * t230;
		double t235 = t29 * t39;
		double t237 = 0.428e0 * t82;
		double t238 = 0.428e0 * t84;
		double t239 = 0.428e0 * t235;
		double t240 = t225 + t231 + t237 + t238 - t239;
		double t241 = t33 * t240;
		double t243 = t18 * t224;
		double t244 = t5 * t230;
		double t245 = t39 * t39;
		double t246 = 0.428e0 * t245;
		double t247 = t52 * t52;
		double t248 = 0.428e0 * t247;
		double t249 = -t243 + t244 + t246 + t248;
		double t250 = t4 * t249;
		double t254 = t147 * t45;
		double t256 = t4 * t240;
		double t257 = t33 * t249;
		double t258 = t45 * t45;
		double t259 = 0.440e0 * t258;
		double t260 = t203 * t203;
		double t261 = 0.440e0 * t260;
		double t263 = t3 * (t256 + t257 + t259 + t261);
		double t265 = t3 * t147;
		double t266 = t43 - t44 + t265;
		double t267 = t207 * t266 / 4;
		double t269 = 0.440e0 * t82;
		double t270 = 0.440e0 * t84;
		double t271 = 0.440e0 * t254;
		double t273 = t3 * (t241 - t250 + t269 + t270 - t271);
		double t279 = t266 * t266 / 4;
		double t282 = t3 * (-0.4845000000e0 * t273 + 0.4845000000e0 * t256 + 0.4845000000e0 * t257 + 0.2131800000e0 * t258 + 0.2885972700e0 * t260 + 0.7541727e-1 * t279);
		double t284 = 0.4863250000e1 * t241 - 0.4863250000e1 * t250 + 0.1038924235e1 * t82 + 0.1038924235e1 * t84 - 0.1001215600e1 * t254 + 0.2422500000e0 * t263 - 0.3770863500e-1 * t267 - t282 / 2;
		double t285 = t33 * t284;
		double t302 = 0.4863250000e1 * t256 + 0.4863250000e1 * t257 + 0.1001215600e1 * t258 + 0.1038924235e1 * t260 + t3 * (0.4845000000e0 * t241 - 0.4845000000e0 * t250 + 0.2885972700e0 * t82 + 0.2885972700e0 * t84 - 0.2131800000e0 * t254 + 0.4845000000e0 * t263 - 0.7541727e-1 * t267) / 2 - 0.2422500000e0 * t273 + 0.3770863500e-1 * t279;
		double t303 = t4 * t302;
		double t304 = 0.9401e1 * t225 + 0.9401e1 * t231 + 0.179718917e1 * t82 + 0.179718917e1 * t84 - 0.179718917e1 * t235 + t285 + t303;
		double t305 = t5 * t304;
		double t312 = -0.9401e1 * t243 + 0.9401e1 * t244 + 0.179718917e1 * t245 + 0.179718917e1 * t247 - t4 * t284 + t33 * t302;
		double t313 = t18 * t312;
		double t314 = t79 - t81 - t222 + t223;
		double t315 = t5 * t314;
		double t317 = t97 + t227 + t228 + t229;
		double t318 = t18 * t317;
		double t323 = t315 + t318 + t237 + t238 - t239;
		double t324 = t33 * t323;
		double t326 = t18 * t314;
		double t327 = t5 * t317;
		double t328 = -t326 + t327 + t246 + t248;
		double t329 = t4 * t328;
		double t334 = t4 * t323;
		double t335 = t33 * t328;
		double t337 = t3 * (t334 + t335 + t259 + t261);
		double t341 = t3 * (t324 - t329 + t269 + t270 - t271);
		double t349 = t3 * (-0.4810000000e0 * t341 + 0.4810000000e0 * t334 + 0.4810000000e0 * t335 + 0.2116400000e0 * t258 + 0.2833859600e0 * t260 + 0.7174596e-1 * t279);
		double t351 = 0.4690500000e1 * t324 - 0.4690500000e1 * t329 + 0.9981399800e0 * t82 + 0.9981399800e0 * t84 - 0.9622670000e0 * t254 + 0.2405000000e0 * t337 - 0.3587298000e-1 * t267 - t349 / 2;
		double t352 = t33 * t351;
		double t369 = 0.4690500000e1 * t334 + 0.4690500000e1 * t335 + 0.9622670000e0 * t258 + 0.9981399800e0 * t260 + t3 * (0.4810000000e0 * t324 - 0.4810000000e0 * t329 + 0.2833859600e0 * t82 + 0.2833859600e0 * t84 - 0.2116400000e0 * t254 + 0.4810000000e0 * t337 - 0.7174596e-1 * t267) / 2 - 0.2405000000e0 * t341 + 0.3587298000e-1 * t279;
		double t370 = t4 * t369;
		double t371 = 0.9622e1 * t315 + 0.9622e1 * t318 + 0.183135526e1 * t82 + 0.183135526e1 * t84 - 0.183135526e1 * t235 + t352 + t370;
		double t372 = t5 * t371;
		double t379 = -0.9622e1 * t326 + 0.9622e1 * t327 + 0.183135526e1 * t245 + 0.183135526e1 * t247 - t4 * t351 + t33 * t369;
		double t380 = t18 * t379;
		double t381 = t95 * t93;
		double t383 = t77 * t104;
		double t385 = t82 + t84;
		double t386 = t57 * t385;
		double t393 = -t77 * t16 - t95 * t1 * t9 - (t134 + t135) * qdot[2];
		double t394 = t76 * t393;
		double t396 = t122 * qdot[3];
		double t398 = t122 * t136;
		double t400 = 0.4515e1 * t381 + 0.4515e1 * t383 + 0.121304235e1 * t386 - 0.121304235e1 * t394 - 0.121304235e1 * t396 - 0.121304235e1 * t398;
		double t401 = t95 * t400;
		double t404 = -t76 * t128 + t57 * t141;
		double t405 = t77 * t404;
		double t406 = t160 * t158;
		double t408 = t157 * t161;
		double t410 = t152 * t385;
		double t417 = -t157 * t16 - t160 * t1 * t9 - (t191 + t192) * qdot[4];
		double t418 = t156 * t417;
		double t420 = t179 * qdot[5];
		double t422 = t179 * t193;
		double t424 = 0.4717e1 * t406 + 0.4717e1 * t408 + 0.125848136e1 * t410 + 0.125848136e1 * t418 + 0.125848136e1 * t420 - 0.125848136e1 * t422;
		double t425 = t160 * t424;
		double t428 = t156 * t185 + t152 * t198;
		double t429 = t157 * t428;
		double t430 = 0.35373879e3 * t78 - 0.35373879e3 * t80 - 0.1247883859e2 * t82 - 0.1247883859e2 * t84 + 0.1247883859e2 * t86 + t305 - t313 + t372 - t380 + t401 - t405 + t425 - t429;
		double t432 = t2 * t215 - t6 * t430;
		double t437 = t18 * t304;
		double t438 = t5 * t312;
		double t439 = t18 * t371;
		double t440 = t5 * t379;
		double t441 = t77 * t400;
		double t442 = t95 * t404;
		double t443 = t157 * t424;
		double t444 = t160 * t428;
		double t445 = 0.35373879e3 * t96 - 0.1247883859e2 * t71 - 0.1247883859e2 * t99 + t437 + t438 + t439 + t440 + t441 + t442 + t443 + t444;
		double t450 = t6 * t215 + t2 * t430;
		double t454 = -t7 * t432 + t1 * t445;
		double t472 = 0.7471731200e1 * t82 + 0.428e0 * t285 + 0.1038924235e1 * t241 - 0.1038924235e1 * t250 - 0.1258363106e0 * t254 + 0.1442986350e0 * t263 - 0.3237591060e-1 * t267 - 0.2200000000e0 * t282 + 0.428e0 * t303 + 0.183135526e1 * t315 + 0.183135526e1 * t318 + 0.428e0 * t352;
		double t478 = -0.3247e-1 * t25 + 0.3247e-1 * t28;
		double t482 = 0.4908886536e0 * t37 + 0.4908886536e0 * t38;
		double t487 = -0.4500000000e-3 * t206 + 0.4500000000e-3 * t145 + 0.4500000000e-3 * t146;
		double t492 = 0.4140406848e-2 * t43 - 0.4140406848e-2 * t44 + 0.4140406848e-2 * t265;
		double t499 = 0.367e-2 * t120 + 0.367e-2 * t121;
		double t504 = 0.4024076219e-1 * t134 + 0.4024076219e-1 * t135 + 0.4024076219e-1 * qdot[3];
		double t510 = 0.125e-2 * t120 + 0.125e-2 * t121;
		double t515 = 0.3380516083e-1 * t134 + 0.3380516083e-1 * t135 + 0.3380516083e-1 * qdot[3];
		double t520 = 0.31e-3 * t120 + 0.31e-3 * t121;
		double t525 = 0.5239019368e-2 * t134 + 0.5239019368e-2 * t135 + 0.5239019368e-2 * qdot[3];
		double t527 = 0.4994459548e0 * t386 - 0.4994459548e0 * t394 - 0.4994459548e0 * t396 + t136 * t499 - t122 * t504 + 0.121304235e1 * t381 + 0.121304235e1 * t383 + t136 * t510 - t122 * t515 - 0.4200810124e0 * t398 + t136 * t520 - t122 * t525;
		double t531 = 0.4927418466e0 * t37 + 0.4927418466e0 * t38;
		double t535 = -0.1920e-1 * t26 + 0.1920e-1 * qdot[11];
		double t539 = 0.2420725027e0 * t22 + 0.2420725027e0 * t23;
		double t543 = 0.691e-2 * t145 + 0.691e-2 * t146;
		double t545 = 0.9981399800e0 * t324 - 0.9981399800e0 * t329 + 0.1416929800e0 * t337 + t39 * t478 - t29 * t482 + t266 * t487 / 2 - t207 * t492 / 2 + t57 * t527 - t29 * t531 + t24 * t535 - t27 * t539 + t45 * t543;
		double t549 = 0.2327117896e0 * t43 - 0.2327117896e0 * t44;
		double t560 = 0.5983432610e-1 * t191 + 0.5983432610e-1 * t192 - 0.5983432610e-1 * qdot[5];
		double t564 = 0.5982432610e-1 * t180 + 0.5982432610e-1 * t181;
		double t569 = 0.2781110355e-1 * t191 + 0.2781110355e-1 * t192 - 0.2781110355e-1 * qdot[5];
		double t573 = 0.2774110355e-1 * t180 + 0.2774110355e-1 * t181;
		double t578 = 0.4666238392e-2 * t191 + 0.4666238392e-2 * t192 - 0.4666238392e-2 * qdot[5];
		double t582 = 0.4906238392e-2 * t180 + 0.4906238392e-2 * t181;
		double t584 = -0.548e-2 * t156 * t385 + 0.548e-2 * t152 * t417 - 0.548e-2 * t182 * qdot[5] + t182 * t560 - t193 * t564 + t182 * t569 - t193 * t573 + t182 * t578 - t193 * t582;
		double t589 = -0.4450000000e-3 * t206 + 0.4450000000e-3 * t145 + 0.4450000000e-3 * t146;
		double t594 = 0.4479863062e-2 * t43 - 0.4479863062e-2 * t44 + 0.4479863062e-2 * t265;
		double t598 = 0.739e-2 * t145 + 0.739e-2 * t146;
		double t602 = 0.2460195162e0 * t43 - 0.2460195162e0 * t44;
		double t606 = -0.24629e0 * t26 + 0.24629e0 * qdot[11];
		double t610 = 0.1000407035e1 * t22 + 0.1000407035e1 * t23;
		double t614 = -0.3447e-1 * t25 + 0.3447e-1 * t28;
		double t619 = -t147 * t549 - t156 * t584 + t266 * t589 / 2 - t207 * t594 / 2 + t45 * t598 - t147 * t602 + t24 * t606 - t27 * t610 + t39 * t614 - 0.2200000000e0 * t349 + 0.428e0 * t370 - 0.479e0 * t401;
		double t634 = 0.4024076219e-1 * t123 - 0.4024076219e-1 * t124;
		double t639 = 0.3375516083e-1 * t123 - 0.3375516083e-1 * t124;
		double t644 = 0.5369019368e-2 * t123 - 0.5369019368e-2 * t124;
		double t646 = 0.523e-2 * t76 * t385 + 0.523e-2 * t57 * t393 + 0.523e-2 * t125 * qdot[3] + t125 * t504 - t136 * t634 + t125 * t515 - t136 * t639 + t125 * t525 - t136 * t644;
		double t653 = -0.7487e-1 * t26 + 0.7487e-1 * qdot[11];
		double t657 = 0.1544703300e0 * t22 + 0.1544703300e0 * t23;
		double t664 = -0.372e-2 * t177 + 0.372e-2 * t178;
		double t671 = -0.141e-2 * t177 + 0.141e-2 * t178;
		double t677 = -0.35e-3 * t177 + 0.35e-3 * t178;
		double t680 = 0.4937392784e0 * t410 + 0.4937392784e0 * t418 + 0.4937392784e0 * t420 + t193 * t664 - t179 * t560 + 0.125848136e1 * t406 + 0.125848136e1 * t408 + t193 * t671 - t179 * t569 - 0.4012676104e0 * t422 + t193 * t677 - t179 * t578;
		double t683 = 0.479e0 * t405 - 0.479e0 * t425 + 0.479e0 * t429 + 0.179718917e1 * t225 + 0.179718917e1 * t231 + t76 * t646 + 0.1224174066e3 * t80 + 0.7471731200e1 * t84 - 0.4573656765e1 * t86 + t24 * t653 - t27 * t657 + t152 * t680 - 0.1224174066e3 * t78;
		double t685 = t472 + t545 + t619 + t683;
		double t700 = 0.4935792784e0 * t166 - 0.4935792784e0 * t169 + 0.4935792784e0 * t174 + t179 * t564 - t182 * t664 - 0.125848136e1 * t154 - 0.125848136e1 * t164 + t179 * t573 - t182 * t671 + 0.4012676104e0 * t183 + t179 * t582 - t182 * t677;
		double t704 = t156 * t680 + t152 * t584;
		double t708 = -0.1192203300e0 * t12 + 0.1192203300e0 * t13;
		double t713 = -0.8987870353e0 * t12 + 0.8987870353e0 * t13;
		double t718 = -0.2420625027e0 * t12 + 0.2420625027e0 * t13;
		double t733 = 0.4993659548e0 * t109 - 0.4993659548e0 * t112 + 0.4993659548e0 * t117 + t122 * t634 - t125 * t499 - 0.121304235e1 * t74 + 0.121304235e1 * t107 + t122 * t639 - t125 * t510 + 0.4200810124e0 * t126 + t122 * t644 - t125 * t520;
		double t737 = -t76 * t527 + t57 * t646;
		double t746 = -0.4927118466e0 * t12 + 0.4927118466e0 * t13 + 0.4927118466e0 * qdot[0];
		double t755 = -0.8390813697e-2 * t12 + 0.8390813697e-2 * t13 + 0.8390813697e-2 * qdot[0] - 0.8390813697e-2 * qdot[1];
		double t756 = t207 * t755 / 2;
		double t758 = t203 * t487;
		double t762 = t3 * (t50 - t143 - t148);
		double t767 = t203 * t492;
		double t768 = t266 * t755 / 2;
		double t782 = -0.2327117896e0 * t12 + 0.2327117896e0 * t13 + 0.2327117896e0 * qdot[0] - 0.2327117896e0 * qdot[1];
		double t789 = 0.1785431462e-1 * t48 + 0.4525934154e0 * t50 + 0.5630767914e0 * t53 + t756 / 2 - t758 / 2 - 0.5630767914e0 * t20 - t3 * (-0.4500000000e-3 * t762 + 0.4500000000e-3 * t32 + 0.4500000000e-3 * t42 + 0.4500000000e-3 * t46 + t767 - t768) / 2 - 0.4525934154e0 * t143 - 0.4525934154e0 * t148 + 0.1290603580e2 * t60 + 0.5630767914e0 * t17 + 0.2020273112e0 * t204 + 0.3156822240e-1 * t208 + t147 * t782 - t203 * t543 + 0.2104959968e0 * t69 + 0.2104959968e0 * t71 + 0.5630767914e0 * t30 + 0.1290603580e2 * t63;
		double t809 = 0.3570862925e-1 * t50 - 0.3570862925e-1 * t143 - 0.3570862925e-1 * t148 + 0.4140406848e-2 * t48 + t756 - t758 + 0.7038278676e0 * t60 + 0.7038278676e0 * t63 + 0.1147935360e-1 * t69 + 0.1147935360e-1 * t71 + 0.3070727088e-1 * t17 - 0.3070727088e-1 * t20 + 0.3070727088e-1 * t30 + 0.3070727088e-1 * t53 + 0.3156822240e-1 * t204;
		double t815 = 0.7135000000e-2 * t32 + 0.7135000000e-2 * t42 + 0.7135000000e-2 * t46 + t203 * t549 - t45 * t782 + t3 * t809 / 2 - 0.2250000000e-3 * t762 + t767 / 2 - t768 / 2;
		double t817 = -0.3447e-1 * t34 - 0.3447e-1 * t36 - 0.3447e-1 * t40 + t52 * t531 - t39 * t746 - t4 * t789 + t33 * t815;
		double t826 = -0.4908686536e0 * t12 + 0.4908686536e0 * t13 + 0.4908686536e0 * qdot[0];
		double t836 = t203 * t594;
		double t841 = -0.9089726124e-2 * t12 + 0.9089726124e-2 * t13 + 0.9089726124e-2 * qdot[0] - 0.9089726124e-2 * qdot[1];
		double t842 = t266 * t841 / 2;
		double t846 = t207 * t841 / 2;
		double t848 = t203 * t589;
		double t856 = -0.2460195162e0 * t12 + 0.2460195162e0 * t13 + 0.2460195162e0 * qdot[0] - 0.2460195162e0 * qdot[1];
		double t867 = 0.1883173093e-1 * t48 + 0.4690414465e0 * t50 + 0.5815211326e0 * t53 - 0.5815211326e0 * t20 - t3 * (-0.4450000000e-3 * t762 + 0.4450000000e-3 * t32 + 0.4450000000e-3 * t42 + 0.4450000000e-3 * t46 + t836 - t842) / 2 + t846 / 2 - t848 / 2 - 0.4690414465e0 * t143 - 0.4690414465e0 * t148 + t147 * t856 - t203 * t598 + 0.1332879044e2 * t60 + 0.5815211326e0 * t17 + 0.2041901994e0 * t204 + 0.3318359880e-1 * t208 - 0.2173910776e0 * t69 - 0.2173910776e0 * t71 + 0.5815211326e0 * t30 + 0.1332879044e2 * t63;
		double t887 = 0.3766346186e-1 * t50 - 0.3766346186e-1 * t143 - 0.3766346186e-1 * t148 + 0.4479863062e-2 * t48 + t846 - t848 + 0.7398434187e0 * t60 + 0.7398434187e0 * t63 - 0.1206676320e-1 * t69 - 0.1206676320e-1 * t71 + 0.3227859156e-1 * t17 - 0.3227859156e-1 * t20 + 0.3227859156e-1 * t30 + 0.3227859156e-1 * t53 + 0.3318359880e-1 * t204;
		double t893 = 0.7612500000e-2 * t32 + 0.7612500000e-2 * t42 + 0.7612500000e-2 * t46 + t203 * t602 - t45 * t856 + t3 * t887 / 2 - 0.2225000000e-3 * t762 + t836 / 2 - t842 / 2;
		double t895 = -0.3247e-1 * t34 - 0.3247e-1 * t36 - 0.3247e-1 * t40 + t52 * t482 - t39 * t826 - t4 * t867 + t33 * t893;
		double t915 = -t52 * t614 + 0.1535363544e-1 * t48 + 0.5630767914e0 * t50 + 0.991391808e0 * t53 - 0.1484133655e1 * t20 - 0.5630767914e0 * t143 - 0.5630767914e0 * t148 + 0.4068885126e2 * t60 + t29 * t746 + 0.1484133655e1 * t17 + 0.5477231560e0 * t204 + 0.3070727088e-1 * t208 + t33 * t789 + t4 * t815 + 0.6636306016e0 * t69 + 0.6636306016e0 * t71 + 0.1484133655e1 * t30 + 0.4068885126e2 * t63;
		double t935 = 0.1613929578e-1 * t48 + 0.5815211326e0 * t50 + 0.1023998560e1 * t53 + t29 * t826 - t52 * t478 + t33 * t867 + t4 * t893 - 0.1514887214e1 * t20 - 0.5815211326e0 * t143 - 0.5815211326e0 * t148 + 0.4110104696e2 * t60 + 0.1514887214e1 * t17 + 0.5653818368e0 * t204 + 0.3227859156e-1 * t208 - 0.6703534672e0 * t69 - 0.6703534672e0 * t71 + 0.1514887214e1 * t30 + 0.4110104696e2 * t63;
		double t937 = -0.5970606633e1 * t10 + t160 * t700 - t157 * t704 + t27 * t708 - t14 * t653 + t27 * t713 - t14 * t606 + t27 * t718 - t14 * t535 + t95 * t733 - t77 * t737 - t18 * t817 - t18 * t895 + t5 * t915 + t5 * t935;
		double t954 = 0.160e0 * t437 + 0.160e0 * t438 - 0.160e0 * t439 - 0.160e0 * t440 + 0.185e0 * t441 + 0.185e0 * t442 - 0.185e0 * t443 - 0.185e0 * t444 + 0.479e0 * t142 + 0.479e0 * t186 - 0.479e0 * t199 - 0.5970606633e1 * t15 + 0.479e0 * t129 - 0.1224174066e3 * t63 + 0.4573656765e1 * t67 - 0.1224174066e3 * t60;
		double t955 = t937 + t954;
		double t970 = -0.34036e0 * t89 + t14 * t657 - t24 * t708 + t18 * t935 + t5 * t895 - 0.160e0 * t305 + 0.160e0 * t313 + t18 * t915 + t5 * t817 + 0.160e0 * t372 - 0.160e0 * t380;
		double t983 = t14 * t610 - t24 * t713 + t77 * t733 + t95 * t737 - 0.185e0 * t401 + 0.185e0 * t405 + t157 * t700 + t160 * t704 + 0.185e0 * t425 - 0.185e0 * t429 + t14 * t539 - t24 * t718;
		double t984 = t970 + t983;
		N1[0] = t1 * t432 + t7 * t445;
		N1[1] = t59 * t450 + t62 * t454;
		N1[2] = -t62 * t450 + t59 * t454;
		N1[3] = -t1 * (t2 * t685 - t6 * t955) - t7 * t984;
		N1[4] = t6 * t685 + t2 * t955;
		N1[5] = t984;

		return N1;
	}
}