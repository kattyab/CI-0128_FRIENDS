<template>
  <aside :class="`${is_expanded && 'is_expanded'}`">

    <div class="logo">
      <img src="../assets/logo.svg" alt="Vue">
    </div>

    <div class="menu-toggle-wrap">
      <button class="menu-toggle" @click="ToggleMenu">
        <span class="material-icons">double_arrow</span>
      </button>
    </div>

    <h3>Menu</h3>
    <div class="menu">
      <router-link class="button" to="/">
        <span class="material-icons">home</span>
        <span class="text">Home</span>
      </router-link>
      <router-link class="button" to="/about">
        <span class="material-icons">group</span>
        <span class="text">Team</span>
      </router-link>
      <router-link class="button" to="/login">
        <span class="material-icons">account_circle</span>
        <span class="text">Login</span>
      </router-link>
</div>
  </aside>
</template>

<script setup>
  import { ref } from 'vue'

  const is_expanded = ref(false)

  const ToggleMenu = () => {
    is_expanded.value = !is_expanded.value
  }

</script>


<style lang="scss" scoped>

  aside {
    display: flex;
    flex-direction: column;
    width: calc(2rem + 32px);
    min-height: 100vh;
    overflow: hidden;
    padding: 1rem;
    min-height: 100vh;
    background-color: var(--dark);
    color: var(--light);
    transition: 0.2s ease-out;

    .logo {
      margin-bottom: 1rem;

      img {
        width: 2rem;
      }
    }

    .menu-toggle-wrap {
      display: flex;
      justify-content: flex-end;
      margin-bottom: 1rem;
      position: relative;
      top: 0;
      transition: 0.2s ease-out;

      .menu-toggle {
        transition: 0.2s ease-out;

        .material-icons {
          font-size: 2rem;
          color: var(--light);
          transition: 0.2s ease-out;
        }

        &:hover {
          .material-icons {
            color: var(--primary);
            transform: translateX(0.5rem)
          }
        }
      }
    }


    h3, .button .text {
      opacity: 0;
      transition: 0.3s ease-out;
    }

    h3 {
      color: var(--grey);
      font-size: 0.875rem;
      margin-bottom: 0.5rem;
      text-transform: uppercase;
    }

    .menu {
      margin: 0 -1rem;

      .button {
        display: flex;
        align-items: center;
        text-decoration: none;
        padding: 0.5rem 1rem;
        transition: 0.2s ease-out;

        .material-icons {
          font-size: 2rem;
          color: var(--light);
          transition: 0.2s ease-out;
          margin-right: 1rem;
        }

        .text {
          color: var(--light);
          transition: 0.2s ease-out;
          padding: 0.5rem;
        }

        &.hover, &.router-link-exact-active{
          background-color: var(--dark-alt);

          .material-icons, .text{
            color: var(--primary);

          }
        }
        &.router-link-exact-active{
          border-right: 5px solid var(--primary);
        }
      }
    }

    &.is_expanded {
      width: var(--sidebar-width);

      .menu-toggle-wrap {
        top: -3rem;

        .menu-toggle {
          transform: rotate(180deg);
        }
      }

      h3, .button .text {
        opacity: 1;
      }

      .button{
        .material-icons{
          margin-right: 1rem;
        }
      }
    }

    @media (max-width: 768px) {
      position: fixed;
      z-index: 99;
    }
  }

</style>

<style lang="scss">
  :root {
    --primary: #4ade80;
    --primary-alt: #22c55e;
    --grey: #64748b;
    --dark: #1e293b;
    --dark-alt: #334155;
    --light: #f1f5f9;
    --sidebar-width: 300px;
  }

  * {
    margin: 0;
    padding: 0;
    box-sizing: border-box;
    font-family: 'DM Sans', sans-serif;
  }

  body {
    background: var(--light);
  }

  button {
    cursor: pointer;
    appearance: none;
    border: none;
    outline: none;
    background: none;
  }

  .app {
    display: flex;


    main {
      flex: 1 1 0;
      padding: 2rem;

      @media (max-width: 1024px) {
        padding-left: 6rem;
      }
    }
  }
</style>
