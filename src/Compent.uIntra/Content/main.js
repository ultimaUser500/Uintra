﻿require('./../App_Plugins/News/news');
require('./../App_Plugins/Events/events');

import initCore from './../App_Plugins/Core/Content/Scripts/Core';
import centralFeed from './../App_Plugins/CentralFeed/centralFeed';
import initSearch from './../App_Plugins/Search/search';
import initActionLinkWithConfirm from "../App_Plugins/Core/Content/scripts/ActionLinkWithConfirm";
import initUsers from './../App_Plugins/Users/users';
import initNotification from './../App_Plugins/Notification/notification';
import initTags from './../App_Plugins/Tagging/tags';
import subscribe from "./../App_Plugins/Subscribe/subscribe";
import initNavigation from './../App_Plugins/Navigation/navigation';
import comment from "./../App_Plugins/Comments/comment";
import initBulletings from './../App_Plugins/Bulletins/bulletins';
import contentPanel from './../App_Plugins/Panels/ContentPanel/contentPanel';
import {} from './../App_Plugins/Likes/likes';

initCore();
centralFeed.init();
initSearch();
initActionLinkWithConfirm();
initUsers();
initNotification();
initTags();
subscribe.initOnLoad();
initNavigation();
comment.init();
contentPanel.init();
initBulletings();