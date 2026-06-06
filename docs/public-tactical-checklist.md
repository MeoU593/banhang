# Public Tactical UI Checklist

## Reference Screen Breakdown
- Shell: fixed dark left sidebar, top command banner, utility toolbar, light tactical workspace, footer.
- Sidebar: dark navy base, subtle olive depth, active item with gold vertical marker and darker selected surface.
- Top banner: deep crimson command strip, centered uppercase title, gold subtitle, insignia marks on both sides.
- Toolbar: white/very light surface, breadcrumb left, date/time center-left, compact search, notification/user cluster right.
- Workspace: light blue-gray base, content width around 96%, large cards separated by tonal surfaces instead of heavy divider lines.
- Notification board: large rounded section surface, four white cards, icon tile per card, two recent entries, small metadata.
- Main content grid: primary column around 68%, right column around 29%, 3% gap.
- News cards: image top, badge overlay, strong title, muted summary, date/action row.
- Aside cards: tinted panels for products/forum, stacked compact cards, small images, status chips.
- Documents block: section panel with table-like rows, uppercase metadata header, minimal dividers.
- Footer: white surface, compact copyright, system name, version chip.

## Core Metrics
- Page width: `min(96%, 1600px)`.
- Mobile content width: `min(94%, 100%)`.
- Sidebar width: `clamp(16rem, 22%, 20rem)`.
- Dashboard grid: `68% / 29% / 3% gap`.
- Filter/list grid: `24% / 73% / 3% gap`.
- Product detail grid: `56% / 41% / 3% gap`.
- Article readable width: `min(100%, 72ch)`.
- Page padding: `clamp(1rem, 2vw, 2rem)` vertical, `clamp(1rem, 2.2vw, 2.5rem)` horizontal.
- Section padding: `clamp(1rem, 1.8vw, 1.75rem)`.
- Card padding: `clamp(.875rem, 1.35vw, 1.5rem)`.
- Gap scale: `.5rem / .75rem / 1rem / 1.5rem / 2rem` via `clamp()`.

## Color Rules
- Base: `#f7f9ff`.
- Workspace: `#edf4ff`.
- Section: `#dbe3ef`.
- Card/input: `#ffffff`.
- Dark shell: `#111827`, `#29313a`.
- Crimson primary: `#2e0001`.
- Crimson hover: `#550005`.
- Red accent: `#b91d20`.
- Gold accent: `#fed65b`, `#ffe088`.
- Olive depth only: `#232900`.
- No legacy blue/green UI colors should render from public markup.

## Coverage Checklist
- Layout: `_Root`, `_Root.Head`, `_ColumnsOne`, `_ColumnsTwo`, `_Header`, Footer.
- Shared: notifications, product box, address partials, print, discount, customer/vendor/category navigation.
- Home: homepage, banner, notification cards, home products/blog/forum components.
- Blog: list, detail, comments, months/tags components.
- Catalog: category/search/list/filter/selectors/vendor/manufacturer/tag/new products.
- Product: simple/grouped detail, price, gallery, attributes, reviews, tags, compare, wishlist/cart, recently viewed.
- Customer: login/register/recovery/MFA/profile/info/avatar/address/change password/GDPR/downloads/gift card.
- Vendor public: apply/info/attributes.
- Directory/Guide/Topic static.
- Boards: index/forum/topic/create/edit/post/move/search/subscriptions/all partials.
- Profile public and private messages.
- Wishlist/back-in-stock/common error/sitemap/store closed.
- DocumentPortal public: list/detail/upload/edit/comment.
- Swiper public widget.

## Audit Status
- Global tactical tokens added.
- Layout shell updated.
- Legacy blue/green hardcoded colors removed from grep targets.
- Legacy utility classes neutralized globally to prevent old theme rendering.
- Builds required after each major pass.
